using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Email : PacketPayload
    {
        public static int DefaultEmailSize = 1000;
        public static Material SaturatedMaterial;

        public static void LoadResources()
        {
            SaturatedMaterial = Resources.Load<Material>("Materials/EmailIndicator");
        }

        public override void Initialize(Color c)
        {
            base.Initialize(c);

            Saturated = new Material(SaturatedMaterial);
            Destaturated = new Material(Saturated);

            SetSaturatedColor(Color);

            if (Size <= 0)
            {
                Size = DefaultEmailSize;
            }
        }
    }
}
