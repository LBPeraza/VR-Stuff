using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InternetGame
{
    public struct PacketSourceInfo
    {
        public List<Packet> QueuedPackets;
        public int NumQueuedPackets;
        public int Capacity;
    }

    public abstract class PacketSourceIndicator : MonoBehaviour
    {
        public PacketProcessor Processor;

        public virtual void LoadResources()
        {

        }

        public virtual void Initialize(PacketProcessor processor)
        {
            LoadResources();

            Processor = processor;

            Processor.OnPacketEnqueued += OnPacketEnqueued;
            Processor.OnPacketDequeued += OnPacketDequeued;
            Processor.OnPendingLinkStarted += OnLinkStarted;
            Processor.OnPacketExpired += OnPacketExpired;
        }

        public void OnDestroy()
        {
            if (Processor != null)
            {
                Processor.OnPacketEnqueued -= OnPacketEnqueued;
                Processor.OnPacketDequeued -= OnPacketDequeued;
                Processor.OnPendingLinkStarted -= OnLinkStarted;
                Processor.OnPacketExpired -= OnPacketExpired;
            }
        }

        public virtual void OnPacketExpired(object source, PacketEventArgs p)
        {

        }

        public virtual void OnPacketEnqueued(object source, PacketEventArgs p)
        {

        }

        public virtual void OnPacketDequeued(object source, PacketEventArgs p)
        {

        }

        public virtual void OnLinkStarted(object source, LinkEventArgs l)
        {

        } 
    }
}

