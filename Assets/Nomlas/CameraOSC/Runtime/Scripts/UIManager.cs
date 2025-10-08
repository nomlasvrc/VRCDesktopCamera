using Klak.Spout;
using TMPro;
using UnityEngine;
using TMPUI = TMPro.TextMeshProUGUI;

namespace CameraOSC
{
    public interface IUIManager
    {
        void SetInfoText(string text);
        void Capture();
        void UpdateSpoutSource();
        float Zoom { set; }
    }

    public class UIManager : MonoBehaviour, IUIManager
    {
        #region Inspector
        [SerializeField] private OscQueryManager oscQueryManager;
        [SerializeField] private SpoutReceiver spoutReceiver;
        [Space]
        [SerializeField] private TMPUI infoText;
        [Space]
        [SerializeField] private TMPUI poseText;
        [SerializeField] private TMPUI zoomText;
        [Space]
        [SerializeField] private TMP_InputField positionXText;
        [SerializeField] private TMP_InputField positionYText;
        [SerializeField] private TMP_InputField positionZText;
        [Space]
        [SerializeField] private TMP_Dropdown selectSourceDropdown;
        #endregion

        #region UI Properties
        public float Zoom
        {
            set
            {
                var v = Mathf.RoundToInt(value);
                if (lastSentZoom == v) return;
                userCamera.Send(UserCamera.FloatEndPoint.Zoom, v);
                lastSentZoom = v;
            }
        }
        #endregion

        #region Properties
        public string sourceName
        {
            get => spoutReceiver.sourceName;
            set => spoutReceiver.sourceName = value;
        }
        #endregion

        #region Private Fields
        private UserCamera userCamera;
        private int lastSentZoom = 0;

        #endregion

        #region Public Methods
        public void SetInfoText(string text)
        {
            infoText.text = text;
        }
        #endregion

        #region UI Callbacks
        public void OnChangePositionX(string value)
        {
            Vector3 pos = userCamera.Position;
            pos.x = float.Parse(value);
            userCamera.Send(pos, userCamera.Rotation);
        }
        public void OnChangePositionY(string value)
        {
            Vector3 pos = userCamera.Position;
            pos.y = float.Parse(value);
            userCamera.Send(pos, userCamera.Rotation);
        }
        public void OnChangePositionZ(string value)
        {
            Vector3 pos = userCamera.Position;
            pos.z = float.Parse(value);
            userCamera.Send(pos, userCamera.Rotation);
        }
        public void Capture()
        {
            userCamera.Send(UserCamera.CameraAction.Capture);
        }

        /// <summary>
        /// Spoutのソースリストを更新
        /// GC Allocが発生するので頻繁に呼ばないこと
        /// </summary>
        public void UpdateSpoutSource() => m_UpdateSpoutSource();
        #endregion

        #region Private Methods
        internal void InitializeUI()
        {
            userCamera = oscQueryManager.userCamera;

            positionXText.text = userCamera.Position.x.ToString("F2");
            positionYText.text = userCamera.Position.y.ToString("F2");
            positionZText.text = userCamera.Position.z.ToString("F2");
        }

        private void m_UpdateSpoutSource()
        {
            var sources = SpoutManager.GetSourceNames();

            if (sources.Length > 0)
            {
                selectSourceDropdown.interactable = true;
                selectSourceDropdown.ClearOptions();
                selectSourceDropdown.AddOptions(new System.Collections.Generic.List<string>(sources));
                int currentIndex = System.Array.IndexOf(sources, spoutReceiver.sourceName);
                if (currentIndex < 0) currentIndex = 0;
                selectSourceDropdown.value = currentIndex;
                spoutReceiver.sourceName = sources[currentIndex];
            }
            else
            {
                selectSourceDropdown.interactable = false;
                selectSourceDropdown.ClearOptions();
                selectSourceDropdown.AddOptions(new System.Collections.Generic.List<string> { "No Source" });
            }
        }

        private void Update()
        {
            UpdateUIText();
            MoveCamera();
        }

        private void UpdateUIText()
        {
            poseText.text = $"Position: {userCamera.Position}\nRotation: {userCamera.Rotation}";
            zoomText.text = $"{Mathf.RoundToInt(userCamera.Zoom)}°";
        }

        private void MoveCamera()
        {
            float dx = Input.GetAxis("Horizontal");
            float dy = Input.GetAxis("Vertical");
            if (dx != 0 || dy != 0)
            {
                Vector3 pos = userCamera.Position;
                pos.x += dx * Time.deltaTime * 5;
                pos.y += dy * Time.deltaTime * 5;
                userCamera.Send(pos, userCamera.Rotation);
            }
        }
        #endregion
    }
}
