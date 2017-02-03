using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class Virus : Packet
    {
        public float Damage;
        public float VirusAlertPercentage;
        public bool VirusHasAlerted;
        public AudioSource AudioSource;
        public AudioClip VirusAlertClip;

        public override void Initialize()
        {
            base.Initialize();

            InitializeAudio();
        }

        public virtual void InitializeAudio()
        {
            AudioSource = AudioMix.AddAudioSourceTo(this.gameObject);

            VirusAlertClip = Resources.Load<AudioClip>("virus_warning");
        }

        public override void OnDequeuedFromPort(PacketSource p, Link l)
        {
            base.OnDequeuedFromPort(p, l);

            l.OnSever += OnSever;
        }

        private void PlayVirusAlertClip()
        {
            AudioSource.Stop();
            AudioSource.clip = VirusAlertClip;
            AudioSource.volume = AudioMix.VirusApproachingSoundEffectVolume;
            AudioSource.loop = true;
            AudioSource.Play();

            VirusHasAlerted = true;
        }

        private void StopAudio()
        {
            AudioSource.Stop();
        }

        public override void OnTransmissionProgress(float percentage)
        {
            if (!VirusHasAlerted && percentage > VirusAlertPercentage)
            {
                PlayVirusAlertClip();

                VirusHasAlerted = true;
            }
        }

        public virtual void OnSever(Link severerd, SeverCause cause, float totalLength)
        {
            StopAudio();

            GameManager.ReportStoppedVirus(this);
        }
        
        public override void OnDequeuedFromLink(Link l, PacketSink p)
        {
            base.OnDequeuedFromLink(l, p);

            OnVirusStrikes(p);
        }

        public virtual void OnVirusStrikes(PacketSink sink)
        {
            GameManager.ReportVirusDelivered(this);

            sink.PlayAudioClip(PacketSinkSoundEffect.VirusStrikes);
        }
    }
}
