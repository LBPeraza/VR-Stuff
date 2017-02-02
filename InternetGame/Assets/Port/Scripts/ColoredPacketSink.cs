using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class ColoredPacketSink : PacketSink
    {
        public Color Color;

        public override void Initialize()
        {
            base.Initialize();

            Color = (Color) PacketSpawner.AddressToColor[this.Address];
            GetComponent<Renderer>().material.color = Color;
        }
    }

}

