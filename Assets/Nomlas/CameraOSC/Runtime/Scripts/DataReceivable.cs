using UnityEngine;

namespace CameraOSC
{
    public abstract class DataReceivable : MonoBehaviour
    {
        internal protected UserCamera userCamera;
        public virtual void InitializeDataReceiver() { }

        public virtual void OnChange() { }
    }
}
