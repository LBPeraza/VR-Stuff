using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PassthroughHopper : PacketHopper
    {
        public override void EnqueuePacket(Packet p)
        {
            base.EnqueuePacket(p);

            Source.EnqueuePacket(p);
        }
    }
}

