using System;
using Mirror;
using UnityEngine;

public class MonsterModel : NetworkBehaviour
{
    [field: Header("Camera rotate settings")]
    [field: SerializeField, Range(0, 120)] public float LookDownAngle { get; private set; } = 60;
    [field: SerializeField, Range(0, 120)] public float LookUpAngle { get; private set; } = 60;
    [field: SerializeField] public float LookSensitivity { get; private set; } = 2f;

    [field: Header("Movement settings")]
    [SyncVar(hook = nameof(OnSpeedChanged))]
    [SerializeField] private float moveSpeed = 5f;
    [field: SerializeField] private float baseMoveSpeed { get; set; } = 5f;

    [field: SerializeField] public bool CanMove = true;
    [field: SerializeField] public bool CanRotate = true;

    [Header("Health settings")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private float health = 100;

    public event Action OnDieEvent;

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
        Vector3 respawnPosition = new Vector3(2.5f, 20, -2);

        // Set position on server first
        transform.position = respawnPosition;

        // Then notify clients
        RpcHandleDeath(respawnPosition);
    }

    [ClientRpc]
    private void RpcTeleportToPosition(Vector3 position)
    {
        Debug.Log("Respawn position = " + position);
        transform.position = position;
    }

    [ClientRpc]
    private void RpcHandleDeath(Vector3 respawnPosition)
    {
        Debug.Log($"<color=green>[Client]</color> RpcHandleDeath called for {gameObject.name}");

        // Set position directly
        transform.position = respawnPosition;

        // Trigger any death effects/animations
        OnDieEvent?.Invoke();

        if (isServer)
        {
            health = 100;
        }
    }

    #endregion

    #region Speed Logic

    //Метод для изменения MoveSpeed с клиента через команду
    [Server]
    public void SetSpeed(float newSpeed)
    {
        if (newSpeed < 0) 
            return;

        moveSpeed = newSpeed;
    }

    [Server]
    public void ResetSpeedToDefault() => moveSpeed = baseMoveSpeed;

    public float GetMoveSpeed() => moveSpeed;

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
    #endregion
}
