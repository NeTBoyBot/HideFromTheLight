using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

public class MonsterModel : NetworkBehaviour
{
    [field: Header("Camera rotate settings")]
    [field: SerializeField, Range(0, 120)] public float LookDownAngle { get; private set; } = 60;
    [field: SerializeField, Range(0, 120)] public float LookUpAngle { get; private set; } = 60;
    [field: SerializeField] public float LookSensitivity { get; private set; } = 2f;


    [Header("Movement settings")]
    [SyncVar(hook = nameof(OnSpeedChanged))]
    [SerializeField] private float _moveSpeed = 5f;
    private float _baseMoveSpeed { get; set; } = 5f;

    [SyncVar]
    [Min(0.1f), SerializeField] private float _speedMultiplier = 1f;
    [field: SerializeField] public bool CanMove { get; private set; } = true;
    [field: SerializeField] public bool CanRotate { get; private set; } = true;


    [field: Header("Movement settings/Unmaterialized form")]
    [field: SerializeField] public LayerMask ExcludeLayers { get; private set; }
    [field: SerializeField] public LayerMask DefaultLayer { get; private set; }
    [field: SyncVar]
    [field: Min(0),SerializeField] public float MaterializeTime { get; private set; }
    [field: SyncVar]
    [field: Min(0), SerializeField] public float UnmaterializeTime { get; private set; }
    [field: Min(0.1f), SerializeField, SyncVar] public float TransformationSlowness { get; private set; }



    [Header("Health settings")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private float health = 100;

    [HideInInspector, SyncVar] public Vector2 InputLook = Vector2.zero;
    [HideInInspector, SyncVar] public Vector3 InputMove = Vector2.zero;

    [HideInInspector] public float CameraPitch = 0;
    [field: HideInInspector] public CharacterController CharacterController { get; private set; } = null;

    [Header("Other settings")]
    [SyncVar] public bool IsTransforming = false;
    private CancellationTokenSource _source = new();
    private MonsterAbilities _abilities;

    public event Action OnDieEvent;
    public event Action<float> onHealthChanged;

    #region Initialize
    private void OnEnable()
    {
        if (_source.IsCancellationRequested || _source == null)
        {
            _source = new CancellationTokenSource();
        }
    }

    private void OnDisable()
    {
        _source?.Cancel();
        _source?.Dispose();
    }

    public void Initialize(MonsterAbilities abilities)
    {
        CharacterController = GetComponentInChildren<CharacterController>();
        _abilities = abilities;
        _baseMoveSpeed = _moveSpeed;
    }

    #endregion

    #region Health&Death Logic

    [Server]
    public void ChangeHealth(float value)
    {
        health += value;

        if(health <= 0)
        {
            health = 0;
            HandleDeath();
        }
    }

    public float GetHealth() => health;

    [Server]
    private void HandleDeath()
    {
        Debug.Log($"<color=red>[Server]</color> HandleDeath called for {gameObject.name}");
        Vector3 respawnPosition = new(2.5f, 20, -2);

        //Set position on server first
        CharacterController.enabled = false;
        transform.position = respawnPosition;
        CharacterController.enabled = true;

        health = 100;

        //Then notify clients
        RpcHandleDeath(respawnPosition);
    }

    [ClientRpc]
    private void RpcHandleDeath(Vector3 respawnPosition)
    {
        Debug.Log($"<color=green>[Client]</color> RpcHandleDeath called for {gameObject.name}");

        //Set position directly
        CharacterController.enabled = false;
        transform.position = respawnPosition;
        CharacterController.enabled = true;

        //Trigger any death effects/animations
        OnDieEvent?.Invoke();
    }

    #endregion

    #region Speed Logic

    //Метод для изменения MoveSpeed с клиента через команду
    [Server]
    public void SetSpeed(float newSpeed)
    {
        if (newSpeed < 0) 
            return;

        _moveSpeed = newSpeed;
    }

    [Server]
    public void ResetSpeedToDefault() => _moveSpeed = _baseMoveSpeed;

    public float GetMoveSpeed(bool withSpeedMultiplier = true)
    {
        if (withSpeedMultiplier)
        {
            return _moveSpeed * _speedMultiplier;
        }

        return _moveSpeed * _speedMultiplier;
    }

    public float GetSpeedMultipler() => _speedMultiplier;
    public void SetSpeedMultiplier(float value) => _speedMultiplier = Mathf.Clamp(value, 0.1f, 10f);
    [ClientRpc]
    public void RpcModifySpeedMultiplier(float delta) => SetSpeedMultiplier(_speedMultiplier + delta);
    [ClientRpc]
    public void RpcResetSpeedMultiplier() => SetSpeedMultiplier(1);
    #endregion

    #region SyncVar Handlers
    //Hooks вызываются и на сервере, и на клиентах
    //Можно использовать для визуальных/звуковых эффектов
    //Хорошо подходят для обновления UI

    //НЕ рекомендуется использовать для:
    //-Игровой логики, которая должна быть только на сервере
    //-Тяжелых вычислений (они будут выполняться при каждом изменении)
    //-Изменения других SyncVar (может вызвать рекурсию)
    private void OnSpeedChanged(float oldValue, float newValue)
    {
        Debug.Log($"<color=yellow>[SpeedSync]</color> changed from {oldValue} to {newValue}");
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        Debug.Log($"<color=yellow>[HealthSync]</color> changed from {oldValue} to {newValue}");
        onHealthChanged?.Invoke(newValue);
    }
    #endregion

    #region Abilities
    //Замедление всех игроков, вынести в монстра
    //[Command]
    //public void CmdChangeSpeedAllPlayers(float newSpeed)
    //{
    //if (!isLocalPlayer)
    //    return;

    //RpcSetAllPlayersSpeed(newSpeed);
    //}
    //[ClientRpc]
    //private void RpcSetAllPlayersSpeed(float newSpeed)
    //{
    //    foreach (var conn in NetworkServer.connections.Values)
    //    {
    //        if(conn.identity != null)
    //        {
    //            var player = conn.identity.GetComponent<PlayerModel>();
    //            player.MoveSpeed = newSpeed;
    //            Debug.Log($"[ClientRpc] MoveSpeed установлено на {newSpeed} для клиента {player.netId}");
    //        }
    //    }
    //}

    //materialized form ability

    public async UniTask<bool> ToggleMaterializationForm (bool canPassThrough)
    {
        Debug.Log("EnterUnmaterializeForm");
        if (IsTransforming)
            return false;

        IsTransforming = true;
        RpcModifySpeedMultiplier(-TransformationSlowness);

        var transformationTime = canPassThrough ? MaterializeTime : UnmaterializeTime;

        await UniTask.Delay(
            TimeSpan.FromSeconds(transformationTime),
            cancellationToken: _source.Token);

        CharacterController.excludeLayers = canPassThrough ? ExcludeLayers : DefaultLayer;

        IsTransforming = false;
        RpcEnterUnmaterializeForm(canPassThrough);

        RpcResetSpeedMultiplier();

        return true;
    }

    [ClientRpc]
    private void RpcEnterUnmaterializeForm(bool canPassThrough)
    {
        CharacterController.excludeLayers = canPassThrough ? ExcludeLayers : DefaultLayer;
    }

    #endregion

    [Server]
    public void Respawn()
    {
        HandleDeath();
    }

    public bool Materialized() => _abilities.Materialized;
}
