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
        [field: SyncVar(hook = nameof(SyncVarSetSpeed))]
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field: SerializeField] private float baseMoveSpeed { get; set; } = 5f;

        [field: SerializeField] public bool CanMove = true;
        [field: SerializeField] public bool CanRotate = true;

        public void SetSpeed(float newSpeed)
        {
            if (newSpeed < 0)
                return;

            MoveSpeed = newSpeed;
        }

        public void SyncVarSetSpeed(float oldValue, float newValue)
        {
            if (newValue < 0)
                return;

            Debug.Log($"MoveSpeed изменено с {oldValue} на {newValue}");
        }

        //Восстановление базовой скорости через сервер
        public void ResetSpeedToDefault() => MoveSpeed = baseMoveSpeed;
    }
}
