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

            Color = (Color) GameUtils.AddressToColor[this.Address];
            GetComponent<Renderer>().material.color = Color;
        }
    }

}

