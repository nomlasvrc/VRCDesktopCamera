using UnityEngine;

namespace CameraOSC
{
    [RequireComponent(typeof(Animator))]
    public class ToggleMenu : MonoBehaviour
    {
        private Animator animator;
        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Toggle() => animator.SetBool("open", !animator.GetBool("open"));
    }
}
