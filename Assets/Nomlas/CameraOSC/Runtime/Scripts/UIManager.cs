using System;
using System.Linq;
using Klak.Spout;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPUI = TMPro.TextMeshProUGUI;

// UI管理クラス

namespace CameraOSC
{
    public class UIManager : DataReceivable
    {
        // ----- Unity関連のコード -----
        #region Inspector
        [SerializeField] private OscQueryManager oscQueryManager;
        [SerializeField] private SpoutReceiver spoutReceiver;
        [Space]
        [SerializeField] private TMPUI infoText;
        [Space]
        [SerializeField] private Slider zoomSlider;
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
                var v = Mathf.RoundToInt(Mathf.Pow(280, value) + 20);
                if (lastSentZoom == v) return;
                userCamera.Send(UserCamera.FloatEndPoint.Zoom, v);
                lastSentZoom = v;
                lastEditZoomTime = Time.time;
            }
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

        public void OnChangeSpout()
        {
            var newSourceName = selectSourceDropdown.options[selectSourceDropdown.value].text;
            SpoutSourceName = newSourceName;
        }

        public void UpdateSpoutSource() => m_UpdateSpoutSource(); // GC Allocが発生するので頻繁に呼ばないこと
        #endregion

        // -------------------------------------------

        #region Public API
        public string SpoutSourceName
        {
            get => spoutReceiver.sourceName;
            set => spoutReceiver.sourceName = value;
        }

        public void SetInfoText(string text)
        {
            infoText.text = text;
        }
        #endregion

        private int lastSentZoom = 0;
        private float lastEditZoomTime;
        private const float zoomEditDelay = 0.4f;

        public override void InitializeDataReceiver()
        {
            base.InitializeDataReceiver();
            lastEditZoomTime = Time.time;
            UpdateSpoutSource();
        }

        private void m_UpdateSpoutSource()
        {
            var sources = SpoutManager.GetSourceNames();

            if (sources.Length > 0)
            {
                selectSourceDropdown.interactable = true;
                selectSourceDropdown.ClearOptions();
                selectSourceDropdown.AddOptions(sources.ToList());
                int currentIndex = Array.IndexOf(sources, SpoutSourceName);
                if (currentIndex < 0) currentIndex = 0;
                selectSourceDropdown.value = currentIndex;
                SpoutSourceName = sources[currentIndex];
            }
            else
            {
                selectSourceDropdown.interactable = false;
                selectSourceDropdown.ClearOptions();
                selectSourceDropdown.AddOptions(new System.Collections.Generic.List<string> { "No Source" });
            }
        }

        public override void OnChange()
        {
            var pos = userCamera.Position;
            positionXText.SetTextWithoutNotify(pos.x.ToString("F2"));
            positionYText.SetTextWithoutNotify(pos.y.ToString("F2"));
            positionZText.SetTextWithoutNotify(pos.z.ToString("F2"));
            
            zoomText.text = $"{Mathf.RoundToInt(userCamera.Zoom)}°";
            if (lastEditZoomTime + zoomEditDelay < Time.time)
            {
                zoomSlider.SetValueWithoutNotify(MathF.Log(userCamera.Zoom - 20, 280));
            }
        }
    }
}
