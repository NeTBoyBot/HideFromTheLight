using Mirror;
using UnityEngine;

namespace Develop.Scripts.Core.Lobby
{
    /// <summary>
    /// Player script in game scene for identification | inherited from NetworkBehaviour
    /// </summary>
    public class PlayerIdentification : NetworkBehaviour
    {
        [SyncVar] public string PlayerName;
        [SyncVar] public int PlayerId;
        [SyncVar] public PlayerRole PlayerRole;
        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log($"OnStartClient {PlayerName}");
        }

        public void Initialize(string name, int id)
        {
            PlayerName = name;
            PlayerId = id;
            PlayerRole = PlayerRole.Human;
        }
    }
}
