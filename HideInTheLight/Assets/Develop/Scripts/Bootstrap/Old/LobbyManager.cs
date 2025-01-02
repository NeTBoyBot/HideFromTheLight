using Develop.Scripts.Core.Lobby.Old;
using Mirror;
using System.Linq;
using UnityEngine;

namespace Develop.Scripts.Bootstrap
{
    public class LobbyManager : NetworkRoomManager
    {
        //[SerializeField] private GameObject humanPrefab;
        //[SerializeField] private GameObject monsterPrefab;
        [SerializeField] public PlayerContainer _container;

        private PlayerCustomize _playerCustomize;

        public void EnableLobbyGUI() => showRoomGUI = true;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            
            print($"CONNECTED ID {conn.connectionId}");

            _playerCustomize = FindObjectOfType<PlayerCustomize>(true); //Maybe null in clients
            _playerCustomize.gameObject.SetActive(true);

            var lobbyPlayer = conn.identity.gameObject.GetComponent<NetworkRoomPlayer>();
            var playerModel = lobbyPlayer.GetComponent<LobbyModel>();

            playerModel.Id = conn.connectionId;
            playerModel.Role = PlayerRole.Human;

            _playerCustomize.Initialize(conn, playerModel, lobbyPlayer, this);
            _container.PlayerRolesInfo.Add(new() { index = conn.connectionId, role = PlayerRole.Human });

            //_playerCustomize.RpcShowCustomizePanel();
        }

        public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log($"Disconnect id = {conn.connectionId}");
            var disconnectedPlayer = _container.PlayerRolesInfo.FirstOrDefault(p => p.index == conn.connectionId);

            if(disconnectedPlayer == null)
            {
                Debug.Log("Disconnected player not found");
                return;
            }

            _container.PlayerRolesInfo.Remove(disconnectedPlayer);

            base.OnRoomServerDisconnect(conn);
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            var roomModel = roomPlayer.GetComponent<LobbyModel>();
            var gameModel = gamePlayer.AddComponent<LobbyModel>();

            gameModel.Id = roomModel.Id;
            gameModel.Role = roomModel.Role;
            gameModel.Name = roomModel.Name;

            return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        }
    }
}

