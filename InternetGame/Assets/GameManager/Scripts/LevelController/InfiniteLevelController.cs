using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class InfiniteLevelController : LevelController
    {
        public float SpawnInterval = 10.0f;
        public float VirusProbability = 0.2f;
        public float LastSpawn;

        public float IntervalIncreaseAmount = 2.0f;
        public float IncreaseSpawnRateInterval = 20.0f;
        public float IncreaseSpawnRateAmount = 1.0f;
        public float LastSpawnRateIncrease;
        public float IncreaseVirusRateInterval = 15.0f;
        public float IncreaseVirusRateAmount = .05f;
        public float LastVirusRateIncrease;

        // Use this for initialization
        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            LastSpawn = GameManager.GetInstance().GameTime();
            LastSpawnRateIncrease = GameManager.GetInstance().GameTime();
            LastVirusRateIncrease = GameManager.GetInstance().GameTime();
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
                        Packet p = PacketFactory.CreateLoadedPacket(
                            Sources[sourceIndex], 
                            Sinks[sinkIndex], 
                            PacketPayloadType.Email);
                    }
                    else
                    {
                        Packet p = PacketFactory.CreateEmailVirus(Sources[sourceIndex], Sinks[sinkIndex]);
                    }

                    LastSpawn = GameManager.GetInstance().GameTime();
                }
            }
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            if (!GameManager.GetInstance().IsPaused)
            {
                float currentTime = GameManager.GetInstance().GameTime();
                if (currentTime > LastSpawn + SpawnInterval)
                {
                    double p = random.NextDouble();
                    if (p > 0.95)
                    {
                        SpawnPacket();
                    }
                }

                if (currentTime > LastSpawnRateIncrease + IncreaseSpawnRateInterval)
                {
                    LastSpawnRateIncrease = currentTime;

                    SpawnInterval -= IncreaseSpawnRateAmount;
                    IncreaseSpawnRateInterval += IntervalIncreaseAmount;
                }

                if (currentTime > LastVirusRateIncrease + IncreaseVirusRateInterval)
                {
                    LastVirusRateIncrease = currentTime;

                    VirusProbability += IncreaseVirusRateAmount;
                    IncreaseVirusRateInterval += IntervalIncreaseAmount;
                }
            }
        }
    }
}
