using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FallingPacket : Packet
    {
        public float FallDuration = 10.0f; // Seconds
        public float AlertPercentage = 0.75f;

        public override void Update()
        {
            if (!IsOnDeck)
            {
                float fallTime = GameManager.GetInstance().GameTime() - EnqueuedTime;
                if (!HasAlerted && fallTime > AlertPercentage * FallDuration)
                {
                    // Alert player to expiring packet.
                    ExpireWarning();

                    HasAlerted = true;
                }
                if (!HasDropped && fallTime >= FallDuration)
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

