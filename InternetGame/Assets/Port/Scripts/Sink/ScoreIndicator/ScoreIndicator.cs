using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class ScoreIndicator : MonoBehaviour
    {
        public virtual void Initialize(PacketSink sink)
        {
            sink.LinkEstablished += OnLinkEstablished;
        }

        protected virtual void OnLinkEstablished(object sender, EstablishedLinkEventArgs e)
        {
            e.Packet.Transmitted += OnPacketTransmitted;
        }

        protected abstract void OnPacketTransmitted(Packet p);
    }
}

