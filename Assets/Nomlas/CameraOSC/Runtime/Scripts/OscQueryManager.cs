using System.Net;
using System.Net.Sockets;
using OscCore;
using UnityEngine;
using VRC.OSCQuery;

namespace CameraOSC
{
    public class OscQueryManager : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;

        private OSCQueryService _oscQuery;
        private OscServer _receiver;
        internal UserCamera userCamera { get; private set; }
        private const string SERVER_NAME = "VRCDesktopCamera";
        internal static int portTCP = 9458;
        internal static int portUDP = 9459;

        void Start()
        {
            StartService();
        }

        internal void StartService()
        {
            portTCP = GetAvailableTcpPort();
            portUDP = GetAvailableUdpPort();
            _receiver = OscServer.GetOrCreate(portUDP);

            userCamera = new UserCamera("127.0.0.1", 9000);

            IDiscovery discovery = new MeaModDiscovery();

            _oscQuery = new OSCQueryServiceBuilder()
                .WithServiceName(SERVER_NAME)
                .WithHostIP(IPAddress.Loopback)
                .WithOscIP(IPAddress.Loopback)
                .WithTcpPort(portTCP)
                .WithUdpPort(portUDP)
                .WithDiscovery(discovery)
                .StartHttpServer()
                .AdvertiseOSC()
                .AdvertiseOSCQuery()
                .Build();

            _oscQuery.RefreshServices();

            // Show server name and chosen port
            uiManager.SetInfoText($"tcp: {portTCP}\nosc: {portUDP}");

            AddMethods();

            uiManager.InitializeUI();
        }

        #region Sub Methods
        private void AddMethods()
        {
            _receiver.TryAddMethod($"/usercamera/Mode", ReadMode);
            _oscQuery.AddEndpoint<int>("/usercamera/Mode", Attributes.AccessValues.ReadWrite);

            _receiver.TryAddMethod($"/usercamera/Pose", ReadPose);
            _oscQuery.AddEndpoint<float>("/usercamera/Pose", Attributes.AccessValues.ReadWrite);

            _oscQuery.AddEndpoint<bool>("/usercamera/Close", Attributes.AccessValues.WriteOnly);
            _oscQuery.AddEndpoint<bool>("/usercamera/Capture", Attributes.AccessValues.WriteOnly);
            _oscQuery.AddEndpoint<bool>("/usercamera/CaptureDelayed", Attributes.AccessValues.WriteOnly);

            for (int i = 0; i < UserCamera.BoolEndPoint_Count; i++)
            {
                var dataType = (UserCamera.BoolEndPoint)i;
                _receiver.TryAddMethod($"/usercamera/{dataType}", (message) => ReadBool(message, dataType));
                _oscQuery.AddEndpoint<bool>($"/usercamera/{dataType}", Attributes.AccessValues.ReadWrite);
            }

            for (int i = 0; i < UserCamera.FloatEndPoint_Count; i++)
            {
                var dataType = (UserCamera.FloatEndPoint)i;
                _receiver.TryAddMethod($"/usercamera/{dataType}", (message) => ReadFloat(message, dataType));
                _oscQuery.AddEndpoint<float>($"/usercamera/{dataType}", Attributes.AccessValues.ReadWrite);
            }
        }

        public void ReadMode(OscMessageValues message)
        {
            var mode = (UserCamera.CameraMode)message.ReadIntElement(0);
            userCamera.SetData(mode);
        }

        public void ReadPose(OscMessageValues message)
        {
            var position = new Vector3(message.ReadFloatElement(0), message.ReadFloatElement(1), message.ReadFloatElement(2));
            var rotation = new Vector3(message.ReadFloatElement(3), message.ReadFloatElement(4), message.ReadFloatElement(5));
            userCamera.SetData(position, rotation);
        }

        public void ReadBool(OscMessageValues message, UserCamera.BoolEndPoint dataType)
        {
            bool value = message.ReadBooleanElement(0);
            userCamera.SetData(dataType, value);
        }

        public void ReadFloat(OscMessageValues message, UserCamera.FloatEndPoint dataType)
        {
            float value = message.ReadFloatElement(0);
            userCamera.SetData(dataType, value);
        }

        private void OnDestroy()
        {
            _receiver.Dispose();
            _oscQuery.Dispose();
        }
        #endregion

        #region Static Methods
        private static readonly IPEndPoint DefaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, port: 0);
        public static int GetAvailableTcpPort()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(DefaultLoopbackEndpoint);
            return ((IPEndPoint)socket.LocalEndPoint).Port;
        }

        public static int GetAvailableUdpPort()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(DefaultLoopbackEndpoint);
            return ((IPEndPoint)socket.LocalEndPoint).Port;
        }
        #endregion
    }
}