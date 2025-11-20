using System.Net;
using System.Net.Sockets;
using OscCore;
using UnityEngine;
using VRC.OSCQuery;

namespace CameraOSC
{
    public class OscQueryManager : MonoBehaviour
    {
        [SerializeField] private DataReceivable[] dataReceivables;
        [SerializeField] private UIManager uiManager;

        private OSCQueryService _oscQuery;
        private OscServer _receiver;
        private UserCamera userCamera;
        private const string SERVER_NAME = "VRCDesktopCamera";
        private static int portTCP = 9458;
        private static int portUDP = 9459;

        private void Start()
        {
            StartService();
        }

        private void StartService()
        {
            portTCP = GetAvailableTcpPort();
            portUDP = GetAvailableUdpPort();

            var serverName = SERVER_NAME;
            var ipAddress = "127.0.0.1";
            var oscPort = 9000;

            // OSCQueryサービスを開始
            StartOSCQueryService(serverName, ipAddress, oscPort, portTCP, portUDP);

            uiManager.SetInfoText($@"OSCQuery Info:
            Server Name: {serverName}
            IP Address: {ipAddress}
            OSC Port: {oscPort}
            TCP Port: {portTCP}
            UDP Port: {portUDP}");

            // エンドポイントとメソッドを追加
            AddEndpointsAndMethods();

            // DataReceivableにUserCameraを設定
            foreach (var receivable in dataReceivables)
            {
                receivable.InitializeDataReceiver();
                receivable.userCamera = userCamera;
                receivable.enabled = true;
            }
        }

        /// <summary>
        /// OSCQueryサービスを開始します。
        /// </summary>
        private void StartOSCQueryService(string serverName, string ipAddress, int oscPort, int portTCP, int portUDP)
        {
            _receiver = OscServer.GetOrCreate(portUDP);

            userCamera = new UserCamera(ipAddress, oscPort);

            IDiscovery discovery = new MeaModDiscovery();

            _oscQuery = new OSCQueryServiceBuilder()
                .WithServiceName(serverName)
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
        }

        private void OnDestroy()
        {
            _receiver.Dispose();
            _oscQuery.Dispose();
        }

        /// <summary>
        /// OSCQueryのエンドポイントとメソッドを追加します。
        /// </summary>
        private void AddEndpointsAndMethods()
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

        #region Read Methods
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
        #endregion

        #region Port Utility
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