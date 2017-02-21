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
        public static int DefaultChameleonVirusSize = 1000;
        public static float DefaultChameleonVirusDamage = 10.0f;

        public override void Initialize()
        {
            base.Initialize();

            this.Size = DefaultChameleonVirusSize;
            this.Damage = DefaultChameleonVirusDamage;

            Saturated = new Material(Resources.Load<Material>("Materials/EmailIndicator"));
            Destaturated = new Material(Saturated);

            SetSaturatedColor(Color);

            StartColor = this.Saturated.color;
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

            float scaledPercentage = Mathf.Clamp01(
                (percentageDone - ColorChangePercentageOffset) / 
                (1.0f - ColorChangePercentageOffset));
            Color currentColor = Color.Lerp(StartColor, EndColor, scaledPercentage);
            this.Saturated.color = currentColor;

            //float distanceDone = percentageDone * TransmittingLink.TotalLength;
            //float distanceSoFar = 0.0f;
            //foreach (LinkSegment segment in TransmittingLink.Segments)
            //{
            //    distanceSoFar += segment.Length;
            //    if (distanceSoFar > distanceDone)
            //    {
            //        break;
            //    }

            //    segment.Saturate(Saturated);
            //}
        }
    }
}
