using UnityEngine;
using TMPUI = TMPro.TextMeshProUGUI;

namespace CameraOSC
{
    public class UIManager : DataReceivable
    {
        [SerializeField] private OscQueryManager oscQueryManager;
        private UserCamera userCamera;
        [Space]
        [SerializeField] private TMPUI poseText;
        [SerializeField] private TMPUI zoomText;

        [SerializeField] private TMPUI positionXText;
        [SerializeField] private TMPUI positionYText;
        [SerializeField] private TMPUI positionZText;
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

        public override void InitializeDataReceiver()
        {
            userCamera = oscQueryManager.userCamera;

            positionXText.text = userCamera.Position.x.ToString("F2");
            positionYText.text = userCamera.Position.y.ToString("F2");
            positionZText.text = userCamera.Position.z.ToString("F2");
        }

        private void Update()
        {
            poseText.text = $"Position: {userCamera.Position}\nRotation: {userCamera.Rotation}";
            zoomText.text = $"{Mathf.RoundToInt(userCamera.Zoom)}°";

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

        public void Capture()
        {
            userCamera.Send(UserCamera.CameraAction.Capture);
        }

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
        private int lastSentZoom = 0;
    }
}
