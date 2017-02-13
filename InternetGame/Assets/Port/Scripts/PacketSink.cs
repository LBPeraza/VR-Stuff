using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTK;

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

        public AudioSource VirusStrikesAudioSource;
		public AudioClip VirusStrikesAudioClip;

        public List<VRTK_SnapDropZone> DropZones;

		private PortInfo info;

        public virtual void Initialize()
        {
            InitializeAudio();

            DropZones = new List<VRTK_SnapDropZone>(
                GetComponentsInChildren<VRTK_SnapDropZone>());
            // Subscribe to all of the drop zone "on snap" events.
            foreach (VRTK_SnapDropZone dropZone in DropZones)
            {
                dropZone.ObjectSnappedToDropZone += ConnectorSnappedToDropZone;
            }
        }

        private void ConnectorSnappedToDropZone(object sender, SnapDropZoneEventArgs e)
        {
            LinkController.GetInstance().EndLink(this);
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
            l.OnTransmissionStarted += OnTransmissionStarted;
            l.OnSever += OnLinkSevered;

            // Inform the connector that it has been snapped into a port.
            l.Connector.OnSnappedToPort(this);
        }

        public virtual void OnTransmissionStarted(Link l, Packet p)
        {
          
        }

        protected virtual void OnLinkSevered(Link severedLink, SeverCause cause, float totalLength)
        {
            ActiveLink = null;

            var dropZone = severedLink.Connector.GetStoredSnapDropZone();
            if (dropZone != null)
            {
                // Free up the port.
                dropZone.ForceUnsnap();
            }
        }

		public PortInfo portInfo {
			get {
				return new PortInfo (
					transform.position,
					transform.rotation);
			}
		}
    }
}

