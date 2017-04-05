using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class WaveDisplay : MonoBehaviour
    {
        public virtual void Initialize(WavePacketSpawner wavePacketSpawner)
        {
            wavePacketSpawner.WaveCleared += OnWaveCleared;
        }

        protected abstract void OnWaveCleared(int currentWave);
    }
}

