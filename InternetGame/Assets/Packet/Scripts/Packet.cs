using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class Packet : MonoBehaviour
    {
        public int Size; // In "bytes".
        public Material Indicator;
        public string Destination;

        public abstract void OnEnqueuedToPort(PacketSource p);
        public abstract void OnDequeuedFromPort(PacketSource p, Link l);
        public abstract void OnDequeuedFromLink(Link l, PacketSink p);
    }
}
