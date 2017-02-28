using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class Virus : PacketPayload
    {
        public float Damage;
        public float VirusAlertPercentage;
        public bool VirusHasAlerted;
        public AudioSource AudioSource;

        public static AudioClip VirusAlertClip;

        public override void Initialize(Color c)
        {
            base.Initialize(c);

            AudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
        }

        public static void LoadResources()
        {
            VirusAlertClip = Resources.Load<AudioClip>("Audio/virus_warning");
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
            base.OnTransmissionProgress(percentage);

            if (!VirusHasAlerted && percentage > VirusAlertPercentage)
            {
                PlayVirusAlertClip();

                VirusHasAlerted = true;
            }
        }

        public override void OnSever(Link severed, SeverCause cause, float totalLength)
        {
            base.OnSever(severed, cause, totalLength);

            StopAudio();

            GameManager.GetInstance().ReportStoppedVirus(this);
        }
        
        public override void OnDequeuedFromLink(Link l, PacketSink p)
        {
            base.OnDequeuedFromLink(l, p);

            OnVirusStrikes(p);
        }

        public virtual void OnVirusStrikes(PacketSink sink)
        {
            GameManager.GetInstance().ReportVirusDelivered(this);

            sink.PlayAudioClip(PacketSinkSoundEffect.VirusStrikes);
        }
    }
}
