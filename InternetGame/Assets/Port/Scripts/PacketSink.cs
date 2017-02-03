using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum PacketSinkSoundEffect
    {
        VirusStrikes
    }

    public class PacketSink : MonoBehaviour
    {
        public string Address;
        public Link ActiveLink;
		public PortInfo info;

        public AudioSource VirusStrikesAudioSource;
        public AudioClip VirusStrikesAudioClip;

        private void Start()
        {
            // TODO: call Initialize from PortSpawner or similar.
            Initialize();
        }

        public virtual void Initialize()
        {
            this.info = new PortInfo(
                this.transform.position,
                this.transform.rotation
            );

            InitializeAudio();
        }

        private void InitializeAudio()
        {
            VirusStrikesAudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
            VirusStrikesAudioClip = Resources.Load<AudioClip>("virus_strikes");
        }

        public void PlayAudioClip(PacketSinkSoundEffect effect)
        {
            AudioSource source = VirusStrikesAudioSource;
            AudioClip clip = VirusStrikesAudioClip;
            float volume = AudioMix.GeneralSoundEffectVolume;
            bool repeat = false; 

            switch (effect)
            {
                case PacketSinkSoundEffect.VirusStrikes:
                    volume = AudioMix.VirusStrikesSoundEffectVolume;
                    clip = VirusStrikesAudioClip;
                    source = VirusStrikesAudioSource;
                    repeat = false;
                    break;
            }

            source.Stop();
            source.clip = clip;
            source.loop = repeat;
            source.Play();
        }

        public virtual void OnBecameOptionForLink(Link l)
        {

        }

        public virtual void OnNoLongerOptionForLink(Link l)
        {

        }

        public virtual void OnLinkEstablished(Link l, PacketSource s)
        {
        }

        public virtual void OnTransmissionStarted(Link l)
        {
            // Listen for sever events.
            l.OnSever += (Link severed, SeverCause cause, float totalLength) =>
            {
                OnTransmissionSevered(cause, l);
            };
        }

        protected virtual void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            ActiveLink = null;
        }
    }
}

