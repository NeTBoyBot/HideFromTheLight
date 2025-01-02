using Mirror;
using UnityEngine;

namespace Develop.Scripts.Entities.Player
{
    public class PlayerView : NetworkBehaviour
    {
        [Header("Player Components")]
        public Camera Camera;
        public AudioListener AudioListener;
        public CharacterController CharacterController;
    }
}