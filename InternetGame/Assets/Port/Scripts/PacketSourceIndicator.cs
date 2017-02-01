using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InternetGame
{
    public struct PacketSourceInfo
    {
        public int NumQueuedPackets;
        public int Capacity;
    }

    public abstract class PacketSourceIndicator : MonoBehaviour
    {
        public abstract void UpdatePacketSourceInfo(PacketSourceInfo info);   
    }
}

