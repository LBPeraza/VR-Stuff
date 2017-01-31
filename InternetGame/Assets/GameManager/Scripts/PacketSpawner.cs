﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSpawner : MonoBehaviour
    {
        public List<PacketSource> Sources;
        public List<PacketSink> Sinks;

        public float SpawnInterval = 1.5f;
        public float LastSpawn;

        private System.Random random;

        public void Initialize()
        {
            Sources = GameManager.AllPacketSources;
            Sinks = GameManager.AllPacketSinks;

            LastSpawn = Time.fixedTime;

            random = new System.Random();
        }

        private void SpawnPacket()
        {
            if (Sources.Count > 0 && Sinks.Count > 0)
            {
                var sourceIndex = random.Next(Sources.Count);
                var sinkIndex = random.Next(Sinks.Count);
                
                Packet p = PacketFactory.CreateEmail(Sources[sourceIndex], Sinks[sinkIndex]);

                LastSpawn = Time.fixedTime;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.fixedTime > LastSpawn + SpawnInterval)
            {
                double p = random.NextDouble();
                if (p > 0.9)
                {
                    SpawnPacket();
                }
            }
        }
    }
}
