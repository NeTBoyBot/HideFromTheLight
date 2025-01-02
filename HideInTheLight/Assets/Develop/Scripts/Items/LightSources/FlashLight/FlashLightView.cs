using System;
using UnityEngine;

namespace Develop.Scripts.Items.LightSources.FlashLight
{
    public class FlashLightView : MonoBehaviour
    {
        private Light _light;

        private void Awake()
        {
            _light = GetComponentInChildren<Light>();
            _light.gameObject.SetActive(false);
        }

        public void TurnOn()
        {
            _light.gameObject.SetActive(true);
        }

        public void TurnOff()
        {
            _light.gameObject.SetActive(false);
        }
    }
}
