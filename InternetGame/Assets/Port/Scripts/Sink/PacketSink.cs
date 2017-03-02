using System;
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

	[Serializable]
	public class SinkInfo : PortInfo {
		public string address;

		public SinkInfo (Vector3 location, Quaternion orientation, string address)
			: base(location, orientation) {
			this.address = address;
		}
	}

    public class PacketSink : MonoBehaviour
	{
        public string Address;
        public Link ActiveLink;

        public AudioSource VirusStrikesAudioSource;
		public AudioClip VirusStrikesAudioClip;

        public List<VRTK_SnapDropZone> DropZones;

		private PortInfo info;

        public virtual void LoadResources()
        {
            VirusStrikesAudioClip = Resources.Load<AudioClip>("Audio/virus_strikes");
        }

        public virtual void Initialize()
        {
            LoadResources();

            VirusStrikesAudioSource = AudioMix.AddAudioSourceTo(this.gameObject);

            DropZones = new List<VRTK_SnapDropZone>(
                GetComponentsInChildren<VRTK_SnapDropZone>());
            // Subscribe to all of the drop zone "on snap" events.
            foreach (VRTK_SnapDropZone dropZone in DropZones)
            {
                dropZone.ObjectSnappedToDropZone += ConnectorSnappedToDropZone;
                dropZone.ObjectEnteredSnapDropZone += ConnectorEnteredDropZone;
            }
        }

        private void ConnectorEnteredDropZone(object sender, SnapDropZoneEventArgs e)
        {
            Connector connector = e.snappedObject.GetComponent<Connector>();
        }

        private void ConnectorSnappedToDropZone(object sender, SnapDropZoneEventArgs e)
        {
            LinkController.GetInstance().EndLink(this);
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
            l.TransmissionStarted += OnTransmissionStarted;
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

		public SinkInfo portInfo {
			get {
				return new SinkInfo (
					transform.position,
					transform.rotation,
					Address);
			}
		}
    }
}

