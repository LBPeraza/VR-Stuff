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

        public bool Silent { get; set; }

        bool reachedPort = false;

        public override void Initialize()
        {
            base.Initialize();

            Silent = false;
        }

        public override void Update()
        {
            if (!reachedPort)
            {
                float fallTime = GameManager.GetInstance().GameTime() - EnqueuedTime;
                if (!HasAlerted && fallTime > AlertPercentage * Patience)
                {
                    if (!Silent)
                    {
                        // Alert player to expiring packet.
                        OnExpireWarning();
                    }

                    HasAlerted = true;
                }
                if (!HasDropped && fallTime >= Patience)
                {
                    // Drop packet.
                    OnReachedPort();

                    reachedPort = true;
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

