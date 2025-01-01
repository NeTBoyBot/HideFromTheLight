using Develop.Scripts.Bootstrap;
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
        [SerializeField] private LobbyView _lobbyView;
        private LobbyManager _lobbyManager;
        
        public static int LocalClientId = -1;


        #region Initialize

        private void Start()
        {
            Debug.Log("START");
            _lobbyView = FindObjectOfType<LobbyView>();
            _lobbyManager = FindObjectOfType<LobbyManager>();
            
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            _lobbyView.HumanButton.onClick.RemoveAllListeners();
            _lobbyView.MonsterButton.onClick.RemoveAllListeners();

            _lobbyView.HumanButton.onClick.AddListener(OnHumanSelected);
            _lobbyView.MonsterButton.onClick.AddListener(OnMonsterSelected);
        }
        #endregion

        #region MainLogic

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            RpcUpdateMonsterButtonState(!ContainsMonsterInPlayers());
        }

        [Command(requiresAuthority = false)]
        public void CmdSelectRole(int playerId,PlayerRole role)
        {
            if (role == PlayerRole.Monster && _lobbyManager._container.PlayerRolesInfo.Any(p => p.role == PlayerRole.Monster))
            {
                _lobbyView.Chat.text += $"\n<color=red>Monster already selected!</color>";
                RpcUpdateMonsterButtonState(false);
                return;
            }

            if (role == PlayerRole.Monster)
            {
                RpcUpdateMonsterButtonState(false);
            }
            if(role == PlayerRole.Human && _lobbyManager._container.PlayerRolesInfo.First(p=>p.index == playerId).role == PlayerRole.Monster)
            {
                RpcUpdateMonsterButtonState(true);
            }

            _lobbyManager._container.PlayerRolesInfo.First(p=>p.index == playerId).role = role;
            var model = FindObjectsOfType<LobbyModel>().FirstOrDefault(i => i.Id == playerId);
            model.Role = role;
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
                    playerIndex = player.index;
                    break;
                }
            }

            _lobbyView.Chat.text += $"\n{playerName}{playerIndex+1} select role - <color=yellow>{role}</color>";
        }

        [ClientRpc]
        void RpcUpdateMonsterButtonState(bool isInteractable) => _lobbyView.MonsterButton.interactable = isInteractable;
        #endregion

        #region Handlers
        public void OnHumanSelected()
        {
            if (!NetworkClient.isConnected)
                return;

            CmdSelectRole(NetworkClient.localPlayer.gameObject.GetComponent<LobbyModel>().Id,PlayerRole.Human);
        }

        public void OnMonsterSelected()
        {
            if (!NetworkClient.isConnected)
                return;

            CmdSelectRole(NetworkClient.localPlayer.gameObject.GetComponent<LobbyModel>().Id,PlayerRole.Monster);
        }
        #endregion

        public bool ContainsMonsterInPlayers() => _lobbyManager._container.PlayerRolesInfo.Any(p => p.role == PlayerRole.Monster);
        public PlayerRole GetPlayerRole(int connectionId) 
            => _lobbyManager._container.PlayerRoles.ContainsKey(connectionId) ? _lobbyManager._container.PlayerRoles[connectionId] 
            : PlayerRole.None;
    }
}
