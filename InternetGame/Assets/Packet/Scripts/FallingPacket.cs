using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FallingPacket : Packet
    {
        public float AlertPercentage = 0.75f;

        public delegate void OnReachedPortHandler(FallingPacket p);
        public OnReachedPortHandler ReachedPort;

        public override void Update()
        {
            if (State == PacketState.Unset)
            {
                float fallTime = GameManager.GetInstance().GameTime() - EnqueuedTime;
                if (!HasAlerted && fallTime > AlertPercentage * Patience)
                {
                    Debug.Log("packet is alerting");
                    // Alert player to expiring packet.
                    OnExpireWarning();

                    HasAlerted = true;
                }
                if (!HasDropped && fallTime >= Patience)
                {
                    Debug.Log("Packet is expiring");
                    // Drop packet.
                    OnReachedPort();

                    HasDropped = true;
                }
            }
        }

        public void ForceExpire()
        {
            Expire();
        }

        protected void OnReachedPort()
        {
            if (ReachedPort != null)
            {
                ReachedPort.Invoke(this);
            }
        }
    }
}

