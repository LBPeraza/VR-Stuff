using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace InternetGame
{
    public class LevelController : MonoBehaviour
    {
        public List<PacketSource> Sources;
        public List<PacketSink> Sinks;

        public GameManager GameManager;
        public bool IsPaused = false;

        protected System.Random random;

        public virtual void Initialize(GameManager manager)
        {
            GameManager = manager;
            Sources = GameManager.AllPacketSources;
            Sinks = GameManager.AllPacketSinks;

            random = new System.Random();
        }

        protected PacketSource GetRandomSource()
        {
            if (Sources.Exists(src => !src.IsFull()))
            {
                var sourceIndex = random.Next(Sources.Count);
                PacketSource source = Sources[sourceIndex];
                while (source.IsFull())
                {
                    sourceIndex = random.Next(Sources.Count);
                    source = Sources[sourceIndex];
                }
                return source;
            }
            return null;
        }

		protected PacketSink GetRandomSink() {
			int sinkIndex = random.Next (Sinks.Count);
			return Sinks [sinkIndex];
		}

		protected PowerupType GetRandomPowerupType() {
			Array types = Enum.GetValues (typeof(PowerupType));
			int length = types.Length;
			int i; PowerupType type;
			do {
				i = random.Next (length);
				type = (PowerupType) i;
			} while (type == PowerupType.Unset);
			return type;
		}

        // Update is called once per frame
        public virtual void Update()
        {
            
        }
    }
}

