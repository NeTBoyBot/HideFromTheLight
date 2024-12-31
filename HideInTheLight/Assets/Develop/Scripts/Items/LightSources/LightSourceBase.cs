using UnityEngine;

namespace Develop.Scripts.Items.LightSources
{
    public abstract class LightSourceBase : MonoBehaviour
    {
        public abstract void TurnOn();

        public abstract void TurnOff();

        public abstract void Interact();
        
        public abstract bool IsOn { get; set; }
    }
}
