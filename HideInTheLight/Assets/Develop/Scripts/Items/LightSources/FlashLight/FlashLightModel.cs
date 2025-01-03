using Mirror;
using System;
using UnityEngine;

namespace Develop.Scripts.Items.LightSources.FlashLight
{
    public class FlashLightModel : NetworkBehaviour
    {
        public event Action OnOutOfCharge;

        [Header("Damage settings")]
        public float LightDamage;
        [Range(0.0001f, 5f)] public float DamageCooldown = 0.1f;
        [HideInInspector] public float _lastDamageTime;

        [Header("Charge settings")]
        [SyncVar(hook = nameof(OnChargeLevelChanged))]
        [SerializeField]
        [Range(0, 100)] private float ChargeLevel = 100;

        public float ChargeLoss = 1;
        [Range(0.0001f, 5f)] public float ChargeLossCooldown = 0.1f;
        [HideInInspector] public float _lastlossTime;

        [SyncVar(hook = nameof(OnBatteryCountChanged))]
        [SerializeField]
        [Min(0)] private int BatteryCount = 2;

        #region Server logic

        [Server]
        public void RestoreChargeLevel() => ChargeLevel = 100;

        [Server]
        public void ChangeChargeLevel(float value)
        {
            ChargeLevel = Mathf.Clamp(ChargeLevel + value, 0, 100);

            if (ChargeLevel <= 0)
            {
                if (HasBattery())
                {
                    RemoveBattery();
                    RestoreChargeLevel();
                }
                else
                {
                    ChargeLevel = 0;
                    RpcTriggerOutOfCharge();
                }
                return;
            }
        }

        [Server]
        public int AddBattery()
        {
            BatteryCount++;
            return BatteryCount;
        }

        [Server]
        public int RemoveBattery()
        {
            BatteryCount = Mathf.Max(0, BatteryCount - 1);
            return BatteryCount;
        }

        #endregion

        [ClientRpc]
        private void RpcTriggerOutOfCharge() => OnOutOfCharge?.Invoke();
        private void OnBatteryCountChanged(int oldValue, int newValue)
        {
            Debug.Log($"<color=yellow>[BatterySync]</color> Battery count changed from {oldValue} to {newValue}");
        }

        private void OnChargeLevelChanged(float oldValue, float newValue)
        {
            //Debug.Log($"<color=yellow>[ChargeSync]</color> Charge level changed from {oldValue:F1} to {newValue:F1}");
        }

        public bool HasBattery() => BatteryCount > 0;
        public bool HasEnergy() => ChargeLevel > 0;
        public float GetChargeLevel() => ChargeLevel;
    }
}
