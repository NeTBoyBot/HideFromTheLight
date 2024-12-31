using Develop.Scripts.Core.Lobby;
using Mirror;
using UnityEngine;

namespace Develop.Scripts.Bootstrap
{
    public class LobbyManager : NetworkRoomManager
    {
        [SerializeField] private GameObject humanPrefab;
        [SerializeField] private GameObject monsterPrefab;
        public PlayerContainer _playerContainer;

        public override void Start() => _playerContainer = GetComponentInChildren<PlayerContainer>();

        public override void OnClientConnect()
        {
            base.OnClientConnect();


            Debug.Log("ON CLIENT CONNECT");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);

            Debug.Log($"Server connect = {conn.connectionId}");
            var playerId = NetworkConnectionToClient.LocalConnectionId;
            _playerContainer.PlayerRoles[playerId] = PlayerRole.Human;
        }

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);

            Debug.Log($"Server room connect = {conn.connectionId}");
        }
    }
}

