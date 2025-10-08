using OscCore;
using UnityEngine;

namespace CameraOSC
{
    public interface IUserCamera
    {
        UserCamera.CameraMode Mode { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }

        void SetData(UserCamera.CameraMode mode);
        void SetData(Vector3 position, Vector3 rotation);
        void SetData(UserCamera.BoolEndPoint dataType, bool value);
        void SetData(UserCamera.FloatEndPoint dataType, float value);
        void Send(UserCamera.CameraMode mode);
        void Send(Vector3 position, Vector3 rotation);
        void Send(UserCamera.CameraAction cameraAction);
        void Send(UserCamera.BoolEndPoint dataType, bool value);
        void Send(UserCamera.FloatEndPoint dataType, float value);
    }

    public class UserCamera : IUserCamera
    {
        // --- Camera Mode ---
        public enum CameraMode
        {
            Off = 0,
            Photo = 1,
            Stream = 2,
            Emoji = 3,
            Multilayer = 4,
            Print = 5,
            Drone = 6
        }
        public CameraMode Mode { get; private set; }

        // --- Camera Pose ---
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }

        // --- Camera Actions ---
        public enum CameraAction
        {
            Close,
            Capture,
            CaptureDelayed
        }

        // --- Camera Toggles ---
        public const int BoolEndPoint_Count = 18;
        public enum BoolEndPoint
        {
            ShowUIInCamera,
            Lock,
            LocalPlayer,
            RemotePlayer,
            Environment,
            GreenScreen,
            SmoothMovement,
            LookAtMe,
            AutoLevelRoll,
            AutoLevelPitch,
            Flying,
            TriggerTakesPhotos,
            DollyPathsStayVisible,
            CameraEars,
            ShowFocus,
            Streaming,
            RollWhileFlying,
            OrientationIsLandscape
        }

        public bool ShowUIInCamera { get; private set; }
        public bool Lock { get; private set; }
        public bool LocalPlayer { get; private set; }
        public bool RemotePlayer { get; private set; }
        public bool Environment { get; private set; }
        public bool GreenScreen { get; private set; }
        public bool SmoothMovement { get; private set; }
        public bool LookAtMe { get; private set; }
        public bool AutoLevelRoll { get; private set; }
        public bool AutoLevelPitch { get; private set; }
        public bool Flying { get; private set; }
        public bool TriggerTakesPhotos { get; private set; }
        public bool DollyPathsStayVisible { get; private set; }
        public bool CameraEars { get; private set; }
        public bool ShowFocus { get; private set; }
        public bool Streaming { get; private set; }
        public bool RollWhileFlying { get; private set; }
        public bool OrientationIsLandscape { get; private set; }

        // --- Camera Sliders ---
        public const int FloatEndPoint_Count = 14;
        public enum FloatEndPoint
        {
            Zoom,
            Exposure,
            FocalDistance,
            Aperture,
            Hue,
            Saturation,
            Lightness,
            LookAtMeXOffset,
            LookAtMeYOffset,
            FlySpeed,
            TurnSpeed,
            SmoothingStrength,
            PhotoRate,
            Duration
        }

        public float Zoom { get; private set; }
        public float Exposure { get; private set; }
        public float FocalDistance { get; private set; }
        public float Aperture { get; private set; }
        public float Hue { get; private set; }
        public float Saturation { get; private set; }
        public float Lightness { get; private set; }
        public float LookAtMeXOffset { get; private set; }
        public float LookAtMeYOffset { get; private set; }
        public float FlySpeed { get; private set; }
        public float TurnSpeed { get; private set; }
        public float SmoothingStrength { get; private set; }
        public float PhotoRate { get; private set; }
        public float Duration { get; private set; }

        // --- OSC Client ---
        private OscClient sender;
        public UserCamera(string ipAddress, int port)
        {
            sender = new OscClient(ipAddress, port);
        }

        #region SetData
        public void SetData(CameraMode mode)
        {
            this.Mode = mode;
        }

        public void SetData(Vector3 position, Vector3 rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }

        public void SetData(BoolEndPoint dataType, bool value)
        {
            switch (dataType)
            {
                case BoolEndPoint.ShowUIInCamera:
                    ShowUIInCamera = value;
                    break;
                case BoolEndPoint.Lock:
                    Lock = value;
                    break;
                case BoolEndPoint.LocalPlayer:
                    LocalPlayer = value;
                    break;
                case BoolEndPoint.RemotePlayer:
                    RemotePlayer = value;
                    break;
                case BoolEndPoint.Environment:
                    Environment = value;
                    break;
                case BoolEndPoint.GreenScreen:
                    GreenScreen = value;
                    break;
                case BoolEndPoint.SmoothMovement:
                    SmoothMovement = value;
                    break;
                case BoolEndPoint.LookAtMe:
                    LookAtMe = value;
                    break;
                case BoolEndPoint.AutoLevelRoll:
                    AutoLevelRoll = value;
                    break;
                case BoolEndPoint.AutoLevelPitch:
                    AutoLevelPitch = value;
                    break;
                case BoolEndPoint.Flying:
                    Flying = value;
                    break;
                case BoolEndPoint.TriggerTakesPhotos:
                    TriggerTakesPhotos = value;
                    break;
                case BoolEndPoint.DollyPathsStayVisible:
                    DollyPathsStayVisible = value;
                    break;
                case BoolEndPoint.CameraEars:
                    CameraEars = value;
                    break;
                case BoolEndPoint.ShowFocus:
                    ShowFocus = value;
                    break;
                case BoolEndPoint.Streaming:
                    Streaming = value;
                    break;
                case BoolEndPoint.RollWhileFlying:
                    RollWhileFlying = value;
                    break;
                case BoolEndPoint.OrientationIsLandscape:
                    OrientationIsLandscape = value;
                    break;
            }
        }

        public void SetData(FloatEndPoint dataType, float value)
        {
            switch (dataType)
            {
                case FloatEndPoint.Zoom:
                    Zoom = value;
                    break;
                case FloatEndPoint.Exposure:
                    Exposure = value;
                    break;
                case FloatEndPoint.FocalDistance:
                    FocalDistance = value;
                    break;
                case FloatEndPoint.Aperture:
                    Aperture = value;
                    break;
                case FloatEndPoint.Hue:
                    Hue = value;
                    break;
                case FloatEndPoint.Saturation:
                    Saturation = value;
                    break;
                case FloatEndPoint.Lightness:
                    Lightness = value;
                    break;
                case FloatEndPoint.LookAtMeXOffset:
                    LookAtMeXOffset = value;
                    break;
                case FloatEndPoint.LookAtMeYOffset:
                    LookAtMeYOffset = value;
                    break;
                case FloatEndPoint.FlySpeed:
                    FlySpeed = value;
                    break;
                case FloatEndPoint.TurnSpeed:
                    TurnSpeed = value;
                    break;
                case FloatEndPoint.SmoothingStrength:
                    SmoothingStrength = value;
                    break;
                case FloatEndPoint.PhotoRate:
                    PhotoRate = value;
                    break;
                case FloatEndPoint.Duration:
                    Duration = value;
                    break;
            }
        }
        #endregion

        #region Send
        public void Send(CameraMode mode)
        {
            sender.Send("/usercamera/Mode", (int)mode);
        }

        public void Send(Vector3 position, Vector3 rotation)
        {
            sender.Send("/usercamera/Pose", position, rotation);
        }

        public void Send(CameraAction cameraAction)
        {
            sender.Send($"/usercamera/{cameraAction}");
        }

        public void Send(BoolEndPoint dataType, bool value)
        {
            sender.Send($"/usercamera/{dataType}", value);
        }

        public void Send(FloatEndPoint dataType, float value)
        {
            sender.Send($"/usercamera/{dataType}", value);
        }
        #endregion
    }
}
