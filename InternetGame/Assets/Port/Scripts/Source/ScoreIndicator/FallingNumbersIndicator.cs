using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FallingNumbersIndicator : ScoreIndicator
    {
        public TextSpawner TextSpawner;

        public override void Initialize(PacketSource source)
        {
            base.Initialize(source);

            if (TextSpawner == null)
            {
                TextSpawner = gameObject.AddComponent<TextSpawner>();
            }
            TextSpawner.Initialize();
        }

        protected override void OnPacketTransmitted(Packet p)
        {
            SpawnFallingText(p);
        }

        private void SpawnFallingText(Packet p)
        {
            TextSpawner.AddText(p.Payload.Size.ToString(), p.Color);
        }
    }
}

