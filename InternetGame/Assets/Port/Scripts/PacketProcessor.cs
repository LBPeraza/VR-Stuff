using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketEventArgs : EventArgs
    {
        public Packet Packet;
    }

    public class LinkEventArgs : EventArgs
    {
        public Link Link;
    }

    public class EstablishedLinkEventArgs : EventArgs
    {
        public Packet Packet;
        public PacketSource Source;
        public PacketSink Sink;
    }

    public interface PacketProcessor
    {
        event EventHandler<PacketEventArgs> OnPacketEnqueued;
        event EventHandler<PacketEventArgs> OnPacketDequeued;
        event EventHandler<LinkEventArgs> OnPendingLinkStarted;
        event EventHandler<PacketEventArgs> OnPacketExpired;

        void EnqueuePacket(Packet p);

        int Capacity { get; set; }
    }
}
