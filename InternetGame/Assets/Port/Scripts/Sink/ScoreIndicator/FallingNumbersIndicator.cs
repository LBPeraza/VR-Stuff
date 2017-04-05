using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FallingNumbersIndicator : ScoreIndicator
    {
        public float TextSpawnerVerticalOffset = 0.08f; // Meters
        public Dictionary<Transform, TextSpawner> TextSpawners;

        public override void Initialize(PacketSink sink)
        {
            base.Initialize(sink);

            TextSpawners = new Dictionary<Transform, TextSpawner>();

            foreach (var zone in sink.DropZones)
            {
                var textSpawner = TextSpawnerFactory.CreateTextSpawner(
                    zone.gameObject,
                    zone.transform.position + (Vector3.up * TextSpawnerVerticalOffset));
                TextSpawners[zone.transform] = textSpawner;
                textSpawner.Initialize();
            }
        }

        protected override void OnPacketTransmitted(Packet p)
        {
            if (!(p.Payload is Virus))
            {
                SpawnFallingText(p);
            }
        }

        private void SpawnFallingText(Packet p)
        {
            var endpoint = p.TransmittingLink.SinkEndpoint;
            TextSpawners[endpoint].AddText(p.Payload.Size.ToString(), p.Color);
        }
    }
}

