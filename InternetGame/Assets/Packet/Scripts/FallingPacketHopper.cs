using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FallingPacketHopper : PacketHopper
    {
        public override void EnqueuePacket(Packet p)
        {
            base.EnqueuePacket(p);

            Queue.Add(p);

            if (p is FallingPacket)
            {
                // If the packet is a Falling Packet, store it and wait for it to fall.
                FallingPacket fp = p as FallingPacket;
                fp.ReachedPort += PacketReachedPort;
            }
            else
            {
                // Otherwise, just pass the packet on to the port.
                Source.EnqueuePacket(p);
            }
        }

        protected void PacketReachedPort(FallingPacket p)
        {
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
