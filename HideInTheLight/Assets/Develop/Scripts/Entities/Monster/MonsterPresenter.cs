using Develop.Scripts.Core.Lobby;
using Mirror;
using UnityEngine;

public class MonsterPresenter : NetworkBehaviour
{
        private MonsterModel _model = null;
        private MonsterView _view = null;
        private PlayerIdentification _identification = null;

        private Vector2 _inputLook = Vector2.zero;
        private Vector3 _inputMove = Vector2.zero;

        [SerializeField] private MeshRenderer _materializedForm;
        [SerializeField] private GameObject _unMaterializedForm;
        [SyncVar] public bool Materialized = true;
        

        private float _cameraPitch;

        private void Start()
        {
            _view = GetComponent<MonsterView>();
            _model = GetComponent<MonsterModel>();
            _identification = GetComponentInParent<PlayerIdentification>();

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

        public void ChangeMaterializationState()
        {
            if (Materialized)
            {
                CmdUnMaterialize();
            }
            else
            {
                CmdMaterialize();
            }
        }

        [Command]
        public void CmdMaterialize() => RpcMaterialize();
        [Command]
        public void CmdUnMaterialize() => RpcUnMaterialize();

        [ClientRpc]
        private void RpcMaterialize()
        {
            
                _unMaterializedForm.SetActive(false);
                _materializedForm.enabled = true;
                Materialized = true;
            
        }
        [ClientRpc]
        private void RpcUnMaterialize()
        {
            
                _unMaterializedForm.SetActive(true);
                _materializedForm.enabled = false;
                Materialized = false;
            
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
            _inputMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            _inputLook.x = Input.GetAxis("Mouse X") * _model.LookSensitivity;
            _inputLook.y = Input.GetAxis("Mouse Y") * _model.LookSensitivity;
        }

        private void HandleMovement()
        {
            if (!_model.CanMove)
                return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangeMaterializationState();
            }
            
            Vector3 move = transform.TransformDirection(_inputMove) * _model.GetMoveSpeed();
            move.y = Physics.gravity.y;

            _view.CharacterController.Move(move * Time.deltaTime);
        }

        private void HandleRotation()
        {
            if (!_model.CanRotate)
                return;

            transform.Rotate(0, _inputLook.x, 0);

            _cameraPitch -= _inputLook.y;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -_model.LookUpAngle, _model.LookDownAngle);

            _view.Camera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
        }
        #endregion
}
