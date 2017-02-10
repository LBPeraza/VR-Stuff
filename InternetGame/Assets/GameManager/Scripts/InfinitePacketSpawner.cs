using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class InfinitePacketSpawner : PacketSpawner
    {
        public float SpawnInterval = 10.0f;
        public float VirusProbability = 0.2f;
        public float LastSpawn;

        public float IncreaseSpawnRateInterval = 20.0f;
        public float IncreaseSpawnRateAmount = 1.0f;
        public float LastSpawnRateIncrease;
        public float IncreaseVirusRateInterval = 15.0f;
        public float IncreaseVirusRateAmount = .05f;
        public float LastVirusRateIncrease;

        private System.Random random;

        // Use this for initialization
        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            LastSpawn = Time.fixedTime;
            LastSpawnRateIncrease = Time.fixedTime;
            LastVirusRateIncrease = Time.fixedTime;

            random = new System.Random();
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
        public override void Update()
        {
            if (Time.fixedTime > LastSpawn + SpawnInterval)
            {
                double p = random.NextDouble();
                if (p > 0.95)
                {
                    SpawnPacket();
                }
            }

            if (Time.fixedTime > LastSpawnRateIncrease + IncreaseSpawnRateInterval)
            {
                LastSpawnRateIncrease = Time.fixedTime;

                SpawnInterval -= IncreaseSpawnRateAmount;
            }

            if (Time.fixedTime > LastVirusRateIncrease + IncreaseVirusRateInterval)
            {
                LastVirusRateIncrease = Time.fixedTime;

                VirusProbability += IncreaseVirusRateAmount;
            }
        }
    }
}
