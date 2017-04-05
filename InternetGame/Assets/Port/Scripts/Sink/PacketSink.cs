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
		public SinkInfo(string address, Vector3 location, Quaternion orientation)
			: base(address, location, orientation) {
		}
	}

    public class PacketSink : MonoBehaviour
	{
        [Header("General Settings")]
        public string Address;
        public List<VRTK_SnapDropZone> DropZones;

        [Header("Score Indicator Settings")]
        public ScoreIndicatorType ScoreIndicatorType;

        public event EventHandler<EstablishedLinkEventArgs> LinkEstablished;

        protected AudioSource VirusStrikesAudioSource;
        protected AudioClip VirusStrikesAudioClip;

        [HideInInspector]
        public Link ActiveLink;

        protected ScoreIndicator scoreIndicator;
        protected PortInfo info;

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

            switch (ScoreIndicatorType)
            {
                case ScoreIndicatorType.Disabled:
                    break;
                case ScoreIndicatorType.FallingText:
                    scoreIndicator = gameObject.AddComponent<FallingNumbersIndicator>();
                    scoreIndicator.Initialize(this);
                    break;
            }
        }

        private void ConnectorEnteredDropZone(object sender, SnapDropZoneEventArgs e)
        {
            Connector connector = e.snappedObject.GetComponent<Connector>();
        }

        private void ConnectorSnappedToDropZone(object sender, SnapDropZoneEventArgs e)
        {
            LinkController.GetInstance().EndLink(this, ((VRTK.PacketDropZone) sender).transform);
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

            if (LinkEstablished != null)
            {
                LinkEstablished.Invoke(this, new EstablishedLinkEventArgs {
                    Packet = l.Packet,
                    Sink = this,
                    Source = s
                });
            }
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
                    Address,
                    transform.position,
					transform.rotation);
			}
		}
    }
}

