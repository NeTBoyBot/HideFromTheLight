using System.Collections;
using System.Collections.Generic;
using Develop.Scripts.Core.Lobby;
using Mirror;
using NaughtyAttributes;
using UnityEngine;

namespace Develop.Scripts.Bootstrap
{
    public class LobbyManager : NetworkRoomManager
    {
        [SerializeField] private GameObject humanPrefab;
        [SerializeField] private GameObject monsterPrefab;
        public PlayerContainer _container;


        public override void OnClientConnect()
        {
            base.OnClientConnect();
            

            Debug.Log("ON CLIENT CONNECT");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            _container = GetComponentInChildren<PlayerContainer>();
            
            var playerId = conn.connectionId;
            print(playerId);
            
            _container.PlayerRoles[playerId] = PlayerRole.Human;

            Debug.Log($"Server connect = {conn.connectionId}");
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            
            print($"CONNECTED ID {conn.connectionId}");
            conn.identity.GetComponent<LobbyModel>().Id = conn.connectionId;
            conn.identity.GetComponent<LobbyModel>().Role = PlayerRole.Human;
            _container.PlayerRolesInfo.Add(new (){index = conn.connectionId,role = PlayerRole.Human}) ;
            
        }

        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            var roomModel = roomPlayer.GetComponent<LobbyModel>();
            var gameModel = gamePlayer.AddComponent<LobbyModel>();
            gameModel.Id = roomModel.Id;
            gameModel.Role = roomModel.Role;
            NetworkServer.Destroy(roomPlayer);
            return base.OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer);
        }

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);

            Debug.Log($"Server room connect = {conn.connectionId}");
        }
    }
}

