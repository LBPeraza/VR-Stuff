using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class ScoreIndicator : MonoBehaviour
    {
        public virtual void Initialize(PacketSource source)
        {
            source.OnPacketEnqueued += OnPacketEnqueued;
        }

        protected virtual void OnPacketEnqueued(object sender, PacketEventArgs e)
        {
            e.Packet.Transmitted += OnPacketTransmitted;
        }

        protected abstract void OnPacketTransmitted(Packet p);
    }
}

