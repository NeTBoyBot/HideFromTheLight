using Mirror;
using UnityEngine;

namespace Develop.Scripts.Core.Lobby
{
    public class LobbyView : NetworkBehaviour
    {
        [Header("Spawn settings")]
        [SyncVar]
        [SerializeField] private Vector3 _firstSpawnPosition;

        [SyncVar]
        [SerializeField] private float _offsetPosition;

        [ClientRpc]
        public void RpcSetSpawnPosition(NetworkRoomPlayer player) 
            => player.transform.position = new Vector3(_firstSpawnPosition.x += _offsetPosition, 0, 0);

    }
}
