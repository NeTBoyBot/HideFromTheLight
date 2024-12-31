using Develop.Scripts.Bootstrap;
using DI;
using Mirror;
using System.Linq;
using UnityEngine;

public enum PlayerRole
{
    None,
    Human,
    Monster
}
namespace Develop.Scripts.Core.Lobby
{
    public class RoleSelector : NetworkBehaviour
    {
        [Inject] private LobbyView _lobbyView;
        private LobbyManager _lobbyManager;

        [SyncVar] private int monsterConnectionId = -1;


        #region Initialize

        private void Start()
        {
            Debug.Log("START");
            SubscribeEvents();
            _lobbyManager = FindObjectOfType<LobbyManager>();
        }


        private void SubscribeEvents()
        {
            _lobbyView.HumanButton.onClick.RemoveAllListeners();
            _lobbyView.MonsterButton.onClick.RemoveAllListeners();

            _lobbyView.HumanButton.onClick.AddListener(OnHumanSelected);
            _lobbyView.MonsterButton.onClick.AddListener(OnMonsterSelected);
        }
        #endregion

        [Command(requiresAuthority = false)]
        public void CmdSelectRole(PlayerRole role)
        {
            if (role == PlayerRole.Monster && _lobbyManager._playerContainer.PlayerRoles.Any(p => p.Value == PlayerRole.Monster))
            {
                _lobbyView.Chat.text += $"\nМонстр уже выбран и";
                return;
            }

            var playerId = NetworkConnectionToClient.LocalConnectionId;

            if (role == PlayerRole.Monster)
            {
                RpcUpdateMonsterButtonState(false);
            }
            if(role == PlayerRole.Human && _lobbyManager._playerContainer.PlayerRoles[playerId] == PlayerRole.Monster)
            {
                RpcUpdateMonsterButtonState(true);
            }

            _lobbyManager._playerContainer.PlayerRoles[playerId] = role;

            RpcUpdateRoleSelection(playerId, role);
        }

        [ClientRpc]
        void RpcUpdateRoleSelection(int netId, PlayerRole role)
        {
            string playerName = "";
            int playerIndex = 0;
            foreach (var player in _lobbyManager.roomSlots)
            {
                if (player.netId == netId)
                {
                    playerName = player.PlayerName;
                    playerIndex = player.index; //Убрать, когда имя будет нормальным из стима например
                    break;
                }
            }

            _lobbyView.Chat.text += $"\n{playerName}{playerIndex+1} выбрал роль <color=yellow>{role}</color>";    
        }

        [ClientRpc]
        void RpcUpdateMonsterButtonState(bool isInteractable) => _lobbyView.MonsterButton.interactable = isInteractable;

        public PlayerRole GetPlayerRole(int connectionId) => _lobbyManager._playerContainer.PlayerRoles.ContainsKey(connectionId) ? _lobbyManager._playerContainer.PlayerRoles[connectionId] : PlayerRole.None;


        public void OnHumanSelected()
        {
            if (!NetworkClient.isConnected)
                return;

            CmdSelectRole(PlayerRole.Human);
        }

        public void OnMonsterSelected()
        {
            if (!NetworkClient.isConnected)
                return;

            CmdSelectRole(PlayerRole.Monster);
        }
    }
}
