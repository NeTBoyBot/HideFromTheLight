using Develop.Scripts.Bootstrap;
using Mirror;
using System.Collections.Generic;
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
        private NetworkRoomManager _roomManager;

        [SyncVar] private int monsterConnectionId = -1;
        private readonly SyncDictionary<int, PlayerRole> playerRoles = new();

        public SyncDictionary<NetworkIdentity, PlayerRole> _players = new();

        #region Initialize
        private void Start()
        {
            SubscribeEvents();
            _roomManager = FindObjectOfType<LobbyManager>();
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
        public void CmdSelectRole(int netId, PlayerRole role)
        {
            if (role == PlayerRole.Monster && monsterConnectionId != -1)
            {
                Debug.Log("ћонстр уже выбран");
                return;
            }

            if (role == PlayerRole.Monster)
            {
                monsterConnectionId = netId;
                RpcUpdateMonsterButtonState(false);
            }
            else if(role == PlayerRole.Human && monsterConnectionId == netId)
            {
                monsterConnectionId = -1;
                RpcUpdateMonsterButtonState(true);
            }

            playerRoles[netId] = role;


            RpcUpdateRoleSelection(netId, role);
        }

        [Command(requiresAuthority = false)]
        public void CmdSelectRole(PlayerRole role)
        {
            Debug.Log("cmd select role");
            if (role == PlayerRole.Monster && _players.Any(p => p.Value == PlayerRole.Monster))
                return;

            NetworkIdentity localIdentity = NetworkClient.localPlayer;

            if (role == PlayerRole.Monster)
            {
                RpcUpdateMonsterButtonState(false);
            }
            if(role == PlayerRole.Human && _players[localIdentity] == PlayerRole.Monster)
            {
                RpcUpdateMonsterButtonState(true);
            }

            _players[localIdentity] = role;
        }

        [ClientRpc]
        void RpcUpdateRoleSelection(int netId, PlayerRole role)
        {
            string playerName = "";
            int playerIndex = 0;
            foreach (var player in _roomManager.roomSlots)
            {
                if (player.netId == netId)
                {
                    playerName = player.PlayerName;
                    playerIndex = player.index; //”брать, когда им€ будет нормальным из стима например
                    break;
                }
            }

            _lobbyView.Chat.text += $"\n{playerName}{playerIndex+1} выбрал роль <color=yellow>{role}</color>";    
        }

        [ClientRpc]
        void RpcUpdateMonsterButtonState(bool isInteractable) => _lobbyView.MonsterButton.interactable = isInteractable;

        public PlayerRole GetPlayerRole(int connectionId) => playerRoles.ContainsKey(connectionId) ? playerRoles[connectionId] : PlayerRole.None;


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
