using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class WaveDisplay : MonoBehaviour
    {
        public virtual void Initialize(WaveLevelController waveLevelController)
        {
            waveLevelController.WaveCleared += OnWaveCleared;
        }

        protected abstract void OnWaveCleared(int currentWave);
    }
}

