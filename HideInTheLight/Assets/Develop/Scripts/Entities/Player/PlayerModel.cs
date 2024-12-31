using Mirror;
using UnityEngine;

namespace Develop.Scripts.Entities.Player
{
    public class PlayerModel : NetworkBehaviour
    {

        [field: Header("Camera rotate settings")]
        [field: SerializeField, Range(0, 120)] public float LookDownAngle { get; private set; } = 60;
        [field: SerializeField, Range(0, 120)] public float LookUpAngle { get; private set; } = 60;
        [field: SerializeField] public float LookSensitivity { get; private set; } = 2f;

        [field: Header("Movement settings")]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;

        [field: SerializeField] public bool CanMove = true;
        [field: SerializeField] public bool CanRotate = true;
    }
}
