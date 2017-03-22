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

        public static float DefaultChameleonVirusDamage = 10.0f;
        public static float DefaultColorChangePercentageOffset = 0.4f;
        public static Material SaturatedMaterial;

        public static void LoadResources()
        {
            SaturatedMaterial = new Material(Resources.Load<Material>("Materials/EmailIndicator"));
        }

        public override void Initialize(Color c)
        {
            base.Initialize(c);

            if (ColorChangePercentageOffset <= 0)
            {
                ColorChangePercentageOffset = DefaultColorChangePercentageOffset;
            }

            Saturated = new Material(SaturatedMaterial);
            Desaturated = new Material(Saturated);

            SetColors(Color);

            StartColor = this.Saturated.color;
        }

        public override void OnTransmissionProgress(float percentageDone)
        {
            base.OnTransmissionProgress(percentageDone);

            float scaledPercentage = Mathf.Clamp01(
                (percentageDone - ColorChangePercentageOffset) / 
                (1.0f - ColorChangePercentageOffset));
            Color currentColor = Color.Lerp(StartColor, EndColor, scaledPercentage);
            this.Saturated.color = currentColor;
        }
    }
}
