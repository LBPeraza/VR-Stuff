using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSpawner : MonoBehaviour
    {
        public List<PacketSource> Sources;
        public List<PacketSink> Sinks;

        public static Hashtable AddressToColor;

        public float SpawnInterval = 10.0f;
        public float VirusProbability = 0.2f;
        public float LastSpawn;

        private System.Random random;
        private static Color[] AddressColors = { Color.green, Color.blue, Color.magenta, Color.yellow };

        public void Initialize()
        {
            Sources = GameManager.AllPacketSources;
            Sinks = GameManager.AllPacketSinks;

            AddressToColor = new Hashtable();
            BuildAddressToColorTable(Sinks);

            LastSpawn = Time.fixedTime;

            random = new System.Random();
        }

        private void BuildAddressToColorTable(List<PacketSink> Sinks)
        {
            int i = 0;
            foreach (var sink in Sinks)
            {
                AddressToColor.Add(sink.Address, AddressColors[i]);
                i++;
            }
        }

        private void SpawnPacket()
        {
            if (Sources.Count > 0 && Sinks.Count > 0)
            {
                var sourceIndex = random.Next(Sources.Count);
                var sinkIndex = random.Next(Sinks.Count);

                if (!Sources[sourceIndex].IsFull())
                {
                    if (random.NextDouble() > VirusProbability)
                    {
                        Packet p = PacketFactory.CreateEmail(Sources[sourceIndex], Sinks[sinkIndex]);
                    }
                    else
                    {
                        Packet p = PacketFactory.CreateEmailVirus(Sources[sourceIndex], Sinks[sinkIndex]);
                    }

                    LastSpawn = Time.fixedTime;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.fixedTime > LastSpawn + SpawnInterval)
            {
                double p = random.NextDouble();
                if (p > 0.95)
                {
                    SpawnPacket();
                }
            }
        }
    }
}

