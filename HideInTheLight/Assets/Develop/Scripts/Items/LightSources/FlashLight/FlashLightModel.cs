using UnityEngine;

namespace Develop.Scripts.Items.LightSources.FlashLight
{
    public class FlashLightModel : MonoBehaviour
    {
        public int BatteryCount = 2;
        [Range(0,100)] public float ChargeLevel = 100;

        public void AddBattery()
        {
            BatteryCount++;
        }

        public void SetChargeLevel(float value)
        {
            if (value > 100 || value < 0)
                return;
            ChargeLevel = value;
        }
    }
}
