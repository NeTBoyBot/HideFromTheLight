using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MonsterModel : NetworkBehaviour
{
    [field: Header("Camera rotate settings")]
        [field: SerializeField, Range(0, 120)] public float LookDownAngle { get; private set; } = 60;
        [field: SerializeField, Range(0, 120)] public float LookUpAngle { get; private set; } = 60;
        [field: SerializeField] public float LookSensitivity { get; private set; } = 2f;

        [field: Header("Movement settings")]
        [field: SyncVar(hook = nameof(SyncVarSetSpeed))] //Синхронизация MoveSpeed Возможно не работает с Field
        [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
        [field: SerializeField] private float baseMoveSpeed { get; set; } = 5f;

        [field: SerializeField] public bool CanMove = true;
        [field: SerializeField] public bool CanRotate = true;

        [field: SyncVar] 
        [field: SerializeField] public float Health = 100;

        public event Action OnDie;

        private void Start()
        {
            OnDie += Die;
        }

        public void AddHealth(float value)
        {
            Health += value;
            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        public void Die()
        {
            transform.position = Vector3.zero;
        }

        //Метод для изменения MoveSpeed с клиента через команду
        public void SetSpeed(float newSpeed)
        {
            if (newSpeed < 0)
                return;

            MoveSpeed = newSpeed;
        }

        //Замедление всех игроков, вынести в монстра
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
        //            Debug.Log($"[ClientRpc] MoveSpeed установлено на {newSpeed} для клиента {player.netId}");
        //        }
        //    }
        //}

        public void SyncVarSetSpeed(float oldValue, float newValue)
        {
            if (newValue < 0)
                return;

            Debug.Log($"MoveSpeed изменено с {oldValue} на {newValue}");
        }

        //Восстановление базовой скорости через сервер
        public void ResetSpeedToDefault() => MoveSpeed = baseMoveSpeed;
}
