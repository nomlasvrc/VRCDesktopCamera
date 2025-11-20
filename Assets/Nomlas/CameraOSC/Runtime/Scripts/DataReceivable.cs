using UnityEngine;

namespace CameraOSC
{
    public abstract class DataReceivable : MonoBehaviour
    {
        internal protected UserCamera userCamera;
        public virtual void InitializeDataReceiver() { }

        public virtual void OnChangeMode() { }
        public virtual void OnChangePose() { }
        public virtual void OnChangeBool() { }
        public virtual void OnChangeFloat() { }
    }
}
