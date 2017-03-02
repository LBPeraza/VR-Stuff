using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum Soundtrack
    {
        DeepDreamMachine,
        EtherealColosseum,
        WelcomeToTheSimulation
    }

    public class BackgroundMusic : MonoBehaviour, ResourceLoadable
    {
        public AudioSource BackgroundMusicSource;

        public AudioClip DeepDreamMachine;
        public AudioClip EtherealColosseum;
        public AudioClip WelcomeToTheSimulation;

        public void Initialize()
        {
            LoadResources();

            BackgroundMusicSource = this.gameObject.AddComponent<AudioSource>();
            BackgroundMusicSource.loop = true;
            BackgroundMusicSource.spatialBlend = 0.0f; // 2D.
            BackgroundMusicSource.volume = AudioMix.BackgroundMusicVolume;
        }

        public void LoadResources()
        {
            DeepDreamMachine = Resources.Load<AudioClip>("deep_dream_machine");
            EtherealColosseum = Resources.Load<AudioClip>("ethereal_colosseum");
            WelcomeToTheSimulation = Resources.Load<AudioClip>("welcome_to_the_simulation");
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
                case Soundtrack.WelcomeToTheSimulation:
                    soundtrack = WelcomeToTheSimulation;
                    break;
            }

            BackgroundMusicSource.Stop();
            BackgroundMusicSource.clip = soundtrack;
            BackgroundMusicSource.Play();
        }
    }
}

