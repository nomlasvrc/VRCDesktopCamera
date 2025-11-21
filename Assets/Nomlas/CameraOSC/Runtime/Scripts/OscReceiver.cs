using System;
using OscCore;
using UnityEngine;

namespace CameraOSC
{
    public class OscReceiver : MonoBehaviour
    {
        private OscServer _receiver;

        internal void Initialize(int portUDP)
        {
            _receiver = OscServer.GetOrCreate(portUDP);
        }

        internal void TryAddMethod(string address, Action<OscMessageValues> read) => _receiver.TryAddMethod(address, read);

        private void OnDestroy()
        {
            _receiver.Dispose();
        }
    }}
