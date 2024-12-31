using Mirror;
using UnityEngine;

namespace Develop.Scripts.Entities.Player
{
    public class PlayerView : NetworkBehaviour
    {
        [Header("Player Components")]
        public Transform CameraTransform;
        public CharacterController CharacterController;
    }
}