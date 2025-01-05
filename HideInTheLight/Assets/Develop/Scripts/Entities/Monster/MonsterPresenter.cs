using Cysharp.Threading.Tasks;
using Develop.Scripts.Core.Lobby;
using Mirror;
using UnityEngine;

public class MonsterPresenter : NetworkBehaviour
{
    private MonsterModel _model = null;
    private MonsterView _view = null;
    private PlayerIdentification _identification = null;
    private MonsterAbilities _abilities = null;

    private void Start()
    {
        Initialize();

        if (isLocalPlayer)
        {
            Debug.Log($"{_identification.PlayerName} <color=cyan> it's YOU</color> " +
                $"and your id from this server = <color=cyan>{_identification.PlayerId}</color>" +
                $"\n your net id = <color=cyan>{netId}</color>");

            _view.Camera.enabled = true;
            _view.AudioListener.enabled = true;
        }
        else
        {
            Debug.Log($"{_identification.PlayerName} <color=yellow> it's OTHER player</color> " +
                $"and his id from this server = <color=cyan>{_identification.PlayerId}</color>" +
                $"\n his net id = <color=cyan>{netId}</color>");
            _view.Camera.enabled = false;
            _view.AudioListener.enabled = false;
        }
    }

    private void Initialize()
    {
        _view = GetComponent<MonsterView>();
        _model = GetComponent<MonsterModel>();
        _identification = GetComponentInParent<PlayerIdentification>();
        _abilities = GetComponent<MonsterAbilities>();

        _model.Initialize(_abilities);
    }

    private void Update()
    {
        if (!isLocalPlayer) //Если это не ты
            return;         //Не пытайся управлять этим игроком

        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    #region Handlers
    private void HandleInput()
    {
        _model.InputMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        _model.InputLook.x = Input.GetAxis("Mouse X") * _model.LookSensitivity;
        _model.InputLook.y = Input.GetAxis("Mouse Y") * _model.LookSensitivity;
    }

    private void HandleMovement()
    {
        if (!_model.CanMove)
            return;

        Vector3 move = transform.TransformDirection(_model.InputMove) * _model.GetMoveSpeed();
        move.y = Physics.gravity.y;

        _model.CharacterController.Move(move * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (!_model.CanRotate)
            return;

        transform.Rotate(0, _model.InputLook.x, 0);

        _model.CameraPitch -= _model.InputLook.y;
        _model.CameraPitch = Mathf.Clamp(_model.CameraPitch, -_model.LookUpAngle, _model.LookDownAngle);

        _view.Camera.transform.localRotation = Quaternion.Euler(_model.CameraPitch, 0, 0);
    }
    #endregion

    #region Rigidbody logic

    public async UniTask<bool> EnterUnmaterializeForm(bool value) => await _model.EnterUnmaterializeForm(value);

    #endregion
}
