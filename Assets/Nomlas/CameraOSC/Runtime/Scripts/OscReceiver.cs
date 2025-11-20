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

        internal void TryAddMethodPair(string address, Action<OscMessageValues> read, Action mainThread)
        {
            _receiver.TryAddMethodPair(address, new OscActionPair(read, mainThread));
        }

        private void OnDestroy()
        {
            _receiver.Dispose();
        }
    }}
