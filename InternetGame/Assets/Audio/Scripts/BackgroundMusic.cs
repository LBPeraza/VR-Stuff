using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum Soundtrack
    {
        DeepDreamMachine,
        EtherealColosseum
    }

    public class BackgroundMusic : MonoBehaviour
    {
        public AudioSource BackgroundMusicSource;

        public AudioClip DeepDreamMachine;
        public AudioClip EtherealColosseum;

        public void Initialize()
        {
            BackgroundMusicSource = this.gameObject.AddComponent<AudioSource>();
            BackgroundMusicSource.loop = true;
            BackgroundMusicSource.spatialBlend = 0.0f; // 2D.
            BackgroundMusicSource.volume = AudioMix.BackgroundMusicVolume;

            LoadAudioAssets();
        }

        private void LoadAudioAssets()
        {
            DeepDreamMachine = Resources.Load<AudioClip>("deep_dream_machine");
            EtherealColosseum = Resources.Load<AudioClip>("ethereal_colosseum");
        }

        public void Pause()
        {
            BackgroundMusicSource.Pause();
        }

        public void Resume()
        {
            BackgroundMusicSource.UnPause();
        }
        
        public void SetBackgroundSoundtrack(Soundtrack track)
        {
            AudioClip soundtrack = DeepDreamMachine;

            switch (track)
            {
                case Soundtrack.DeepDreamMachine:
                    soundtrack = DeepDreamMachine;
                    break;
                case Soundtrack.EtherealColosseum:
                    soundtrack = EtherealColosseum;
                    break;
            }

            BackgroundMusicSource.Stop();
            BackgroundMusicSource.clip = soundtrack;
            BackgroundMusicSource.Play();
        }
    }
}

