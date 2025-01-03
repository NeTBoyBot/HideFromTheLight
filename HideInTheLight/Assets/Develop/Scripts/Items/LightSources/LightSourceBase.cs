using Mirror;
using UnityEngine;

namespace Develop.Scripts.Items.LightSources
{
    public abstract class LightSourceBase : NetworkBehaviour
    {
        public abstract void CmdTurnOn();

        public abstract void CmdTurnOff();

        public abstract void Interact();
        
        public abstract bool IsOn { get; set; }
    }
}
