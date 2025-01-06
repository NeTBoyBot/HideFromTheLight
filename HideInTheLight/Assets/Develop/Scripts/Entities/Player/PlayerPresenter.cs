using Develop.Scripts.Core.Lobby;
using Develop.Scripts.Interfaces;
using Develop.Scripts.Items.LightSources.FlashLight;
using Mirror;
using UnityEngine;

namespace Develop.Scripts.Entities.Player
{
    public class PlayerPresenter : NetworkBehaviour
    {
        private PlayerModel _model = null;
        private PlayerView _view = null;
        private PlayerIdentification _identification = null;

        private IInteractable _activeItem;

        private void Start()
        {
            _activeItem = GetComponentInChildren<FlashLightPresenter>();
            _view = GetComponent<PlayerView>();
            _model = GetComponent<PlayerModel>();
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

        private void Update()
        {
            if (!isLocalPlayer) //Если это не ты
                return;         //Не пытайся управлять этим игроком

            HandleInput();
            HandleMovement();
            HandleRotation();

            HandleInteractions();
            HandleMenuInput();



            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _activeItem.Interact();
            }
        }

        #region Interact Handlers
        private void HandleInteractions()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _activeItem.Interact();
            }
        }

        private void HandleMenuInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _view.ToggleMenu(_identification);
            }
        }

        #region Movement Handlers
        private void HandleInput()
        {
            _model.InputMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            _model.InputLook.x = Input.GetAxis("Mouse X") * _model.LookSensitivity;
            _model.InputLook.y = Input.GetAxis("Mouse Y") * _model.LookSensitivity;
        }

        private void HandleMovement()
        {
            if (!_model.CanMove)
                return;

            Vector3 move = transform.TransformDirection(_model.InputMove) * _model.MoveSpeed;
            move.y = Physics.gravity.y;

            _view.CharacterController.Move(move * Time.deltaTime);
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
        #endregion
    }

}
