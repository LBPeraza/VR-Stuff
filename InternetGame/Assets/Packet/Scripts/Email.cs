using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Email : Packet
    {
        public override void Initialize()
        {
            base.Initialize();
            Saturated = new Material(Resources.Load<Material>("EmailIndicator"));
            Destaturated = new Material(Saturated);

            SetSaturatedColor(Color);

            this.Size = 1000;
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
