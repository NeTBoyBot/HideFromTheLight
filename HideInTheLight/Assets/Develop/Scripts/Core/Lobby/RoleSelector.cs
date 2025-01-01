using System;
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
        [SerializeField] private LobbyView _lobbyView;
        private LobbyManager _lobbyManager;

        [SyncVar] private int monsterConnectionId = -1;
        
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

        [Command(requiresAuthority = false)]
        public void CmdSelectRole(int playerId,PlayerRole role)
        {
            if (role == PlayerRole.Monster && _lobbyManager._container.PlayerRolesInfo.Any(p => p.role == PlayerRole.Monster))
            {
                _lobbyView.Chat.text += $"\n������ ��� ������ �";
                return;
            }
            print($"ID{playerId}");
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
                    playerIndex = player.index; //������, ����� ��� ����� ���������� �� ����� ��������
                    break;
                }
            }

            _lobbyView.Chat.text += $"\n{playerName}{playerIndex+1} ������ ���� <color=yellow>{role}</color>";    
        }

        [ClientRpc]
        void RpcUpdateMonsterButtonState(bool isInteractable) => _lobbyView.MonsterButton.interactable = isInteractable;

        public PlayerRole GetPlayerRole(int connectionId) => _lobbyManager._container.PlayerRoles.ContainsKey(connectionId) ? _lobbyManager._container.PlayerRoles[connectionId] : PlayerRole.None;


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
    }
}
