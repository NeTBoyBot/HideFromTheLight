using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Scripts.Core.Lobby
{
    public class LobbyView : NetworkBehaviour
    {
        public event Action<string, NetworkConnectionToClient> OnNameAssigned;

        [SerializeField] public Image NicknamePanel;
        [SerializeField] public Image LobbyPanel;
        [SerializeField] public TMP_InputField NicknameField;

        [TargetRpc]
        public void TargetRpcNicknamePanelSetActive(NetworkConnectionToClient conn, bool value) => NicknamePanel.gameObject.SetActive(value);

        [TargetRpc]
        public void TargetRpcLobbyPanelSetActive(NetworkConnectionToClient conn, bool value)
        {
            NicknamePanel.gameObject.SetActive(false);
            LobbyPanel.gameObject.SetActive(value);
        }

        [TargetRpc]
        public void TargetRpcSetupInput(NetworkConnectionToClient conn)
        {
            NicknameField.onEndEdit.RemoveAllListeners();
            NicknameField.onEndEdit.AddListener((name) => OnNicknameEntered(name, conn));
        }

        //Обрботка ввода никнейма
        private void OnNicknameEntered(string nickname, NetworkConnectionToClient conn)
        {
            if (string.IsNullOrWhiteSpace(nickname) || !NetworkClient.isConnected)
                return;

            //Передаем команду на сервер для присвоения имени
            CmdSetupNicknameInput(nickname, conn);
        }

        //Команда для передачи никнейма на сервер
        [Command(requiresAuthority = false)]
        public void CmdSetupNicknameInput(string nickname, NetworkConnectionToClient conn)
        {
            //Отправляем на сервер и присваиваем имя игроку
            var roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();
            if (roomPlayer != null && !string.IsNullOrWhiteSpace(nickname))
            {
                nickname = nickname.Trim();

                roomPlayer.SetName(nickname);

                OnNameAssigned?.Invoke(nickname, conn);

                Debug.Log($"Assigned name <color=yellow>{nickname}</color> to player with id: <color=cyan>{roomPlayer.index}</color>");
            }
            else
            {
                Debug.LogError("<color=red>Failed to assign nickname.</color>");
            }
        }

        private void OnDestroy() => OnNameAssigned = null;
    }
}

