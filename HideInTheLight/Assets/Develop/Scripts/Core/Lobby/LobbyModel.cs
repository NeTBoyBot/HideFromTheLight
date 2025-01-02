using Mirror;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Develop.Scripts.Core.Lobby
{
    public class LobbyModel : NetworkBehaviour
    {
        public event Action<string, NetworkConnectionToClient> OnNameAssigned;

        [Header("Connect settings")]
        [SerializeField] public Image NicknamePanel;

        [Header("Lobby settings")]
        [SerializeField] public Image LobbyPanel;
        [SerializeField] public TMP_InputField NicknameField;
        [SerializeField] public Button HumanSelectButton, MonsterSelectButton;

        [TargetRpc]
        public void TargetRpcNicknamePanelSetActive(NetworkConnectionToClient conn, bool value)
        {
            LobbyPanel.gameObject.SetActive(false);
            NicknamePanel.gameObject.SetActive(value);
        }

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
            HumanSelectButton.onClick.RemoveAllListeners();
            MonsterSelectButton.onClick.RemoveAllListeners();

            NicknameField.onEndEdit.AddListener((name) => OnNicknameEntered(name, conn));
            HumanSelectButton.onClick.AddListener(() => CmdHumanSelectBtn(conn));
            MonsterSelectButton.onClick.AddListener(() => CmdMonsterSelectBtn(conn));
        }

        [Command(requiresAuthority = false)]
        public void CmdHumanSelectBtn(NetworkConnectionToClient conn)
        {
            var roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();

            var monsterConnection = HasMonsterInRoom();

            if (monsterConnection && roomPlayer.RoleName == "Monster")
            {
                roomPlayer.SetRoleName("Human");
                RpcMonsterSelectBtnSetActive(true);
                return;
            }

            roomPlayer.SetRoleName("Human");

            RpcConsoleDebug($"{roomPlayer.Name} Selected Human");
        }
        [Command(requiresAuthority = false)]
        public void CmdMonsterSelectBtn(NetworkConnectionToClient conn)
        {
            var roomPlayer = conn.identity.GetComponent<NetworkRoomPlayer>();

            var monsterConnection = HasMonsterInRoom();

            if (monsterConnection)
                return;

            roomPlayer.SetRoleName("Monster");
            RpcMonsterSelectBtnSetActive(false);

            RpcConsoleDebug($"{roomPlayer.Name} Selected Monster");
        }

        private bool HasMonsterInRoom() => NetworkServer.connections.Values
            .Any(m => m.identity.GetComponent<NetworkRoomPlayer>().RoleName == "Monster");

        [ClientRpc]
        private void RpcConsoleDebug(string message)
        {
            Debug.Log(message);
        }

        [ClientRpc]
        public void RpcMonsterSelectBtnSetActive(bool value) => MonsterSelectButton.interactable = value;


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
                roomPlayer.ShowRoomGUI(true);

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

