using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class ChameleonVirus : Virus
    {
        public Color StartColor;
        public Color EndColor = Color.red;

        public float ColorChangePercentageOffset;

        public void Initialize()
        {
            this.Size = 1000;
            this.Damage = 10.0f;
            this.Indicator = new Material(Resources.Load<Material>("EmailIndicator"));

            StartColor = this.Indicator.color;
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
        }

        public override void OnTransmissionProgress(float percentageDone)
        {
            float scaledPercentage = Mathf.Clamp01((percentageDone - ColorChangePercentageOffset) / (1.0f - ColorChangePercentageOffset));
            Color currentColor = Color.Lerp(StartColor, EndColor, scaledPercentage);
            this.Indicator.color = currentColor;
        }
    }
}
