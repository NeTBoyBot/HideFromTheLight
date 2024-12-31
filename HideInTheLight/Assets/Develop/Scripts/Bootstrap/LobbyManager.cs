using Mirror;
using UnityEngine;

namespace Develop.Scripts.Bootstrap
{
    public class LobbyManager : NetworkRoomManager
    {
        public override void OnRoomClientConnect()
        {
            base.OnRoomClientConnect();

            Debug.Log($"Client room connect = {clientIndex}");
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();

            Debug.Log($"Client connect = {clientIndex}"); 
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);

            Debug.Log($"Server connect = {conn.connectionId}");
        }

        public override void OnRoomServerConnect(NetworkConnectionToClient conn)
        {
            base.OnRoomServerConnect(conn);

            Debug.Log($"Server room connect = {conn.connectionId}");
        }
    }
}

