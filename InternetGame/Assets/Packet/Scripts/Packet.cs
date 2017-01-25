using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class Packet : MonoBehaviour
    {
        public int Size; // In "bytes".
        public Material Indicator;

        public abstract void OnEnqueuedToPort(Port p);
        public abstract void OnDequeuedFromPort(Port p, Link l);
        public abstract void OnDequeuedFromLink(Link l, Port p);
    }
}
