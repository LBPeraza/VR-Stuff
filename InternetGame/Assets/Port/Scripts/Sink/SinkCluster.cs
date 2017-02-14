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

