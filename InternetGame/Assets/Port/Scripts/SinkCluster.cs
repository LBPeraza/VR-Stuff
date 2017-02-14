using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class SinkCluster : PacketSink
    {
        public Color Color;
        public GameObject Backing;

        public override void Initialize()
        {
            base.Initialize();

            if (Backing != null)
            {
                Color = (Color)PacketSpawner.AddressToColor[this.Address];
                Backing.GetComponent<Renderer>().material.color = Color;
            }
        }
    }
}

