using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class SimpleVirus : Virus
    {
        public Color SimpleVirusColor = Color.red;

        public float ColorChangePercentageOffset;

        public static Material SaturatedMaterial;

        public static void LoadResources()
        {
            SaturatedMaterial = new Material(Resources.Load<Material>("Materials/EmailIndicator"));
        }

        public override void Initialize(Color c)
        {
            base.Initialize(c);

            Saturated = new Material(SaturatedMaterial);
            Desaturated = new Material(Saturated);

            SetSaturatedColor(SimpleVirusColor);
            SetDesaturatedColor(MakeLighter(c));
        }

        public override void OnDequeuedFromLink(Link l, PacketSink p)
        {
            base.OnDequeuedFromLink(l, p);
        }
    }
}
