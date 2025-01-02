using System.Collections;
using System.Collections.Generic;
using Develop.Scripts.Core.Lobby;
using Develop.Scripts.Entities.Player;
using Mirror;
using Mirror.Examples.CCU;
using UnityEngine;

public class MonsterPresenter : NetworkBehaviour
{
        private MonsterModel _model = null;
        private MonsterView _view = null;
        private PlayerIdentification _identification = null;

        private Vector2 _inputLook = Vector2.zero;
        private Vector3 _inputMove = Vector2.zero;

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

        private void Update()
        {
            if (!isLocalPlayer) //Если это не ты
                return;         //Не пытайся управлять этим игроком

            HandleInput();
            HandleMovement();
            HandleRotation();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Confined;
                _model.ResetSpeedToDefault();
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                Cursor.lockState = CursorLockMode.Locked;
                _model.SetSpeed(1);
            }
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

            Vector3 move = transform.TransformDirection(_inputMove) * _model.MoveSpeed;
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
