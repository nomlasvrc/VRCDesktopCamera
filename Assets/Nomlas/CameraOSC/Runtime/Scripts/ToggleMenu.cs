using UnityEngine;

namespace CameraOSC
{
    /// <summary>
    /// SubMenuの開閉を制御するクラス
    /// </summary>
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
