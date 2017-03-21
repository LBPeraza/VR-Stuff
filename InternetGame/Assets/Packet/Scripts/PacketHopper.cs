using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketHopper : PacketProcessor
    {
        protected List<Packet> Queue;
        protected PacketSource Source;

        protected PacketSourceIndicator Indicator;

        public int Capacity
        {
            get; set;
        }

        public event EventHandler<PacketEventArgs> OnPacketDequeued;
        public event EventHandler<PacketEventArgs> OnPacketEnqueued;
        public event EventHandler<PacketEventArgs> OnPacketExpired;
        public event EventHandler<LinkEventArgs> OnPendingLinkStarted;

        public virtual void Initialize(PacketSource source, PacketSourceIndicator indicator = null)
        {
            Source = source;

            Queue = new List<Packet>();

            Indicator = indicator;
            if (Indicator != null)
            {
                Indicator.Initialize(this);
            }
        }

        public virtual void EnqueuePacket(Packet p)
        {
            if (OnPacketEnqueued != null)
            {
                OnPacketEnqueued.Invoke(this, new PacketEventArgs { Packet = p });
            }

            p.OnEnqueuedToPort(Source);
        }
    }
}
