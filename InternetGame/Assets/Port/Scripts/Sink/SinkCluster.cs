using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class SinkCluster : PacketSink
    {
        [Header("Basic")]
        public GameObject Backing;
		public GameObject[] Ports;

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

			var packetColor = (Color)PacketSpawner.AddressToColor[this.Address];

            if (Backing != null)
			{
				var originalColor = Backing.GetComponent<Renderer>().material.color;
                Color = Blend(packetColor, originalColor);
                Backing.GetComponent<Renderer>().material.color = Color;
            }

			foreach (GameObject port in Ports) {
				port.GetComponent<Renderer> ().materials [1].color = packetColor;
			}
        }
    }
}

