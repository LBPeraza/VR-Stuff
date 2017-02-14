using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class SinkCluster : PacketSink
    {
        [Header("Basic")]
        public GameObject Backing;

        [HideInInspector]
        public Color Color;

        private Color Blend(Color a, Color b)
        {
            return new Color(
                (a.r + b.r) / 2,
                (a.g + b.g) / 2,
                (a.b + b.b) / 2,
                (a.a + b.a) / 2);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (Backing != null)
            {
                var packetColor = (Color)PacketSpawner.AddressToColor[this.Address];
                var originalColor = Backing.GetComponent<Renderer>().material.color;
                Color = Blend(packetColor, originalColor);
                Backing.GetComponent<Renderer>().material.color = Color;
            }
        }
    }
}

