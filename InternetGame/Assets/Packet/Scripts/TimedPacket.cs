using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class TimedPacket : Packet
    {
        public override void Update()
        {
            if (IsOnDeck)
            {
                if (!HasAlerted && Time.fixedTime > OnDeckTime + AlertTime)
                {
                    // Alert player to expiring packet.
                    ExpireWarning();

                    HasAlerted = true;
                }
                if (!HasDropped && Time.fixedTime > OnDeckTime + Patience)
                {
                    // Drop packet.
                    Expire();

                    HasDropped = true;
                    IsOnDeck = false;
                }
            }
        }
    }
}

