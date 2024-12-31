using UnityEngine;

namespace Develop.Scripts.Interfaces
{
    public interface IDetector<out T> where T : MonoBehaviour
    {
        public T Detect();
    
        public float Radius { get; set; }
        public float Height { get; set; }
        public Vector3 Center { get; set; }
    }
}
