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
        public PacketSource Source;

        public virtual void LoadResources()
        {

        }

        public virtual void Initialize(PacketSource source)
        {
            LoadResources();

            Source = source;

            source.OnPacketEnqueued += OnPacketEnqueued;
            source.OnPacketDequeued += OnPacketDequeued;
            source.OnPendingLinkStarted += OnLinkStarted;
            source.OnPacketExpired += OnPacketExpired;
        }

        public void OnDestroy()
        {
            Source.OnPacketEnqueued -= OnPacketEnqueued;
            Source.OnPacketDequeued -= OnPacketDequeued;
            Source.OnPendingLinkStarted -= OnLinkStarted;
            Source.OnPacketExpired -= OnPacketExpired;
        }

        public virtual void OnPacketExpired(Packet p)
        {

        }

        public virtual void OnPacketEnqueued(Packet p)
        {

        }

        public virtual void OnPacketDequeued(Packet p)
        {

        }

        public virtual void OnLinkStarted(Link l)
        {

        } 
    }
}

