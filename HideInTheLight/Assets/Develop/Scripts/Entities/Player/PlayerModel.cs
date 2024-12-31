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
        [field: SyncVar(hook = nameof(SyncVarSetSpeed))] //������������� MoveSpeed
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field: SerializeField] private float _baseMoveSpeed { get; set; } = 5f;

        [field: SerializeField] public bool CanMove = true;
        [field: SerializeField] public bool CanRotate = true;

        //����� ��� ��������� MoveSpeed � ������� ����� �������
        public void SetSpeed(float newSpeed)
        {
            if (newSpeed < 0)
                return;

            MoveSpeed = newSpeed;
        }

        //���������� ���� �������, ������� � �������
        //[Command]
        //public void CmdChangeSpeedAllPlayers(float newSpeed)
        //{
        //if (!isLocalPlayer)
        //    return;

        //RpcSetAllPlayersSpeed(newSpeed);
        //}
        //[ClientRpc]
        //private void RpcSetAllPlayersSpeed(float newSpeed)
        //{
        //    foreach (var conn in NetworkServer.connections.Values)
        //    {
        //        if(conn.identity != null)
        //        {
        //            var player = conn.identity.GetComponent<PlayerModel>();
        //            player.MoveSpeed = newSpeed;
        //            Debug.Log($"[ClientRpc] MoveSpeed ����������� �� {newSpeed} ��� ������� {player.netId}");
        //        }
        //    }
        //}

        public void SyncVarSetSpeed(float oldValue, float newValue)
        {
            if (newValue < 0)
                return;

            Debug.Log($"MoveSpeed �������� � {oldValue} �� {newValue}");
        }

        //�������������� ������� �������� ����� ������
        public void ResetSpeedToDefault() => MoveSpeed = _baseMoveSpeed;
    }
}
