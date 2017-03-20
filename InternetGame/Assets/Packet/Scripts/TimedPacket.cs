using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class TimedPacket : Packet
    {
        public override void Update()
        {
            if (State == PacketState.OnDeck)
            {
                float currentTime = GameManager.GetInstance().GameTime();
                if (!HasAlerted && currentTime > OnDeckTime + AlertTime)
                {
                    // Alert player to expiring packet.
                    OnExpireWarning();

                    HasAlerted = true;
                }
                if (!HasDropped && currentTime > OnDeckTime + Patience)
                {
                    // Drop packet.
                    Expire();

                    HasDropped = true;
                }
            }
        }
    }
}

