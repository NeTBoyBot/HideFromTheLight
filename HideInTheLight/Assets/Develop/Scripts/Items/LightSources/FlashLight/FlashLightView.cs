using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Threading;
using UnityEngine;

namespace Develop.Scripts.Items.LightSources.FlashLight
{
    public class FlashLightView : NetworkBehaviour
    {
        private Light _light;

        private void Awake()
        {
            _light = GetComponentInChildren<Light>();
            _light.gameObject.SetActive(false);
        }

        [ClientRpc]
        public void RpcTurnOn()
        {
            _light.gameObject.SetActive(true);
        }

        [ClientRpc]
        public void RpcTurnOff()
        {
            _light.gameObject.SetActive(false);
        }

    }
}
