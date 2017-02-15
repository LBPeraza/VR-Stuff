using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Email : Packet
    {
        public static int DefaultEmailSize = 1000;
        public override void Initialize()
        {
            base.Initialize();
            Saturated = new Material(Resources.Load<Material>("Materials/EmailIndicator"));
            Destaturated = new Material(Saturated);

            SetSaturatedColor(Color);

            if (Size <= 0)
            {
                Size = DefaultEmailSize;
            }
        }

        public override void OnDequeuedFromLink(Link l, PacketSink p)
        {
            base.OnDequeuedFromLink(l, p);
        }

        public override void OnDequeuedFromPort(PacketSource p, Link l)
        {
            base.OnDequeuedFromPort(p, l);
        }

        public override void OnEnqueuedToPort(PacketSource p)
        {
            base.OnEnqueuedToPort(p);
        }

        public override void OnTransmissionProgress(float percentageDone)
        {
            base.OnTransmissionProgress(percentageDone);
        }
    }
}
