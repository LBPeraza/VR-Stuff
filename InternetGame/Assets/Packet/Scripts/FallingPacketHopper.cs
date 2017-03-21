using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FallingPacketHopper : PacketHopper
    {
        // Since Source does not get subscribed to packet events,
        // we need to control the light manually.
        protected FlashingLight Light;
        protected float FastPacketTotalDuration = 1.0f; // Seconds

        public override void Initialize(PacketSource source, PacketSourceIndicator Indicator = null)
        {
            base.Initialize(source, Indicator);

            Source.LinkEstablished += LinkEstablished; ;
            Light = source.gameObject.GetComponent<FlashingLight>();
        }

        private void LinkEstablished(object sender, EstablishedLinkEventArgs e)
        {
            if (Queue.Count > 0 && Source.IsEmpty())
            {
                // Fast forward the closest packet.
                FallingPacket p = Queue[0] as FallingPacket;
                FastForwardPacket(p);
            }
        }

        private void OnLinkStarted(object sender, LinkEventArgs e)
        {
            if (Queue.Count > 0 && Source.IsEmpty())
            {
                // Fast forward the closest packet.
                FallingPacket p = Queue[0] as FallingPacket;
                FastForwardPacket(p);
            }
        }

        private void FastForwardPacket(FallingPacket p)
        {
            float elapsedTime = GameManager.GetInstance().GameTime() - p.EnqueuedTime;
            float progress = elapsedTime / p.Patience;
            float timeLeft = (1.0f - progress) * FastPacketTotalDuration;

            p.Patience = elapsedTime + timeLeft;
            p.Silent = true;

            if (Indicator is LightbarIndicator)
            {
                var lightbarIndicator = Indicator as LightbarIndicator;
                LightChunk lc = lightbarIndicator.GetLightChunkFor(p);
                lc.ChangeTimeLeft(timeLeft);
            }
        }

        public override void EnqueuePacket(Packet p)
        {
            base.EnqueuePacket(p);
            
            Queue.Add(p);

            if (p is FallingPacket)
            {
                // If the packet is a Falling Packet, store it and wait for it to fall.
                FallingPacket fp = p as FallingPacket;

                fp.ReachedPort += PacketReachedPort;
                fp.ExpireWarning += PacketAlerted;

                if (Queue.Count == 1 && Source.IsEmpty())
                {
                    FastForwardPacket(fp);
                }
            }
            else
            {
                // Otherwise, just pass the packet on to the port.
                Source.EnqueuePacket(p);
            }
        }

        protected void PacketAlerted(Packet p)
        {
            if (Light != null)
            {
                Light.SetFlashing(true);
            }
        }

        protected void PacketReachedPort(FallingPacket p)
        {
            if (Light != null)
            {
                Light.SetFlashing(false);
            }

            if (Source.IsEmpty())
            {
                // If there is room, enqueue the packet.
                Source.EnqueuePacket(p);
            }
            else
            {
                // Otherwise, drop it.
                p.ForceExpire();
            }

            Queue.Remove(p);
        }
    }
}
