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

        [SerializeField] private GameObject MonsterObject, HumanObject;
        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log($"OnStartClient <color=yellow>{PlayerName} [{PlayerRole}]</color>");
            name = $"{PlayerName} [{PlayerRole}]";

            if(PlayerRole == PlayerRole.Monster)
            {
                MonsterObject.SetActive(true);
                //Destroy(HumanObject);
            }
            else
            {
                HumanObject.SetActive(true);
                //Destroy(MonsterObject);
            }
        }

        public void Initialize(string name, int id, string role)
        {
            PlayerName = name;
            PlayerId = id;
            if(System.Enum.TryParse(role, out PlayerRole parsedRole))
            {
                PlayerRole = parsedRole;
            }
            else
            {
                Debug.LogError($"Invalid role: {role}");
            }
        }
    }
}
