using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum PacketSourceSoundEffect
    {
        PacketWarning,
        PacketDropped,
        PacketEnqueued
    }

    [Serializable]
    public class SourceInfo : PortInfo
    {
        public SourceInfo(Vector3 location, Quaternion orientation)
            : base(location, orientation)
        {
        }
    }

    public class PacketSource : MonoBehaviour, ResourceLoadable
    {
        public GameObject PacketContainer;
        public List<Packet> QueuedPackets;
        public List<Link> ActiveLinks;
        public GameObject IndicatorPrefab;
        public PacketSourceIndicator Indicator;
        public int Capacity = 5;
        public PacketSourceInfo Info;
        public Transform LinkConnectionPoint;
        public Connector Connector;

        public delegate void OnPacketEnqueuedHandler(Packet p);
        public event OnPacketEnqueuedHandler OnPacketEnqueued;
        public delegate void OnPacketDequeuedHandler(Packet p);
        public event OnPacketDequeuedHandler OnPacketDequeued;
        public delegate void OnLinkStartedHandler(Link l);
        public event OnLinkStartedHandler OnPendingLinkStarted;
        public delegate void OnPacketExpiredHandler(Packet p);
        public event OnPacketExpiredHandler OnPacketExpired;

        public AudioSource EnqueuedAudioSource;
        public AudioSource PacketDroppedAudioSource;
        public AudioSource PacketWarningAudioSource;

        public AudioClip PacketWarningClip;
        public AudioClip PacketDroppedClip;
        public AudioClip PacketEnqueuedClip;

        private SourceInfo info;

        public bool HasUnfinishedLink()
        {
            return ActiveLinks.Exists(link => !link.Finished);
        }

        public virtual void LoadResources()
        {
            if (PacketWarningClip == null)
            {
                PacketWarningClip = Resources.Load<AudioClip>("Audio/packet_alert");
            }

            if (PacketDroppedClip == null)
            {
                PacketDroppedClip = Resources.Load<AudioClip>("Audio/packet_dropped");
            }

            if (PacketEnqueuedClip == null)
            {
                PacketEnqueuedClip = Resources.Load<AudioClip>("Audio/packet_enqueued");
            }

            IndicatorPrefab = Resources.Load<GameObject>("Prefabs/RingIndicator");

            Hexagon.LoadResources();
        }

        public virtual void Initialize()
        {
            LoadResources();

            ActiveLinks = new List<Link>();
            QueuedPackets = new List<Packet>();

            Info.Capacity = Capacity;
            Info.QueuedPackets = QueuedPackets;
            Info.NumQueuedPackets = 0;

            if (PacketContainer == null)
            {
                PacketContainer = new GameObject("Prefabs/PacketContainer");
                PacketContainer.transform.parent = this.transform;
            }

            if (LinkConnectionPoint == null)
            {
                LinkConnectionPoint = this.transform;
            }

            if (Indicator == null)
            {
                var indicator = Instantiate(IndicatorPrefab, this.transform, false);
                Indicator = indicator.GetComponent<PacketSourceIndicator>();
            }

            InitializeAudio();

            Indicator.Initialize(this);
        }

        public virtual void InitializeAudio()
        {
            EnqueuedAudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
            PacketDroppedAudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
            PacketWarningAudioSource = AudioMix.AddAudioSourceTo(this.gameObject);
        }

        public void PlayClip(PacketSourceSoundEffect effect)
        {
            float volume = AudioMix.GeneralSoundEffectVolume;
            AudioClip clip = PacketEnqueuedClip;
            AudioSource source = PacketWarningAudioSource;
            bool repeat = false;

            switch (effect)
            {
                case PacketSourceSoundEffect.PacketDropped:
                    source = PacketDroppedAudioSource;
                    clip = PacketDroppedClip;
                    volume = AudioMix.PacketExpiresSoundEffectVolume;
                    break;
                case PacketSourceSoundEffect.PacketEnqueued:
                    source = EnqueuedAudioSource;
                    clip = PacketEnqueuedClip;
                    volume = AudioMix.PacketArrivesSoundEffectVolume;
                    break;
                case PacketSourceSoundEffect.PacketWarning:
                    source = PacketWarningAudioSource;
                    clip = PacketWarningClip;
                    volume = AudioMix.PacketNearingExpirationSoundEffectVolume;
                    break;
            }

            source.Stop();
            source.clip = clip;
            source.volume = volume;
            source.loop = repeat;
            source.Play();
        }

        public bool IsEmpty()
        {
            return QueuedPackets.Count == 0;
        }

        public bool IsFull()
        {
            return QueuedPackets.Count == Capacity;
        }

        public void EnqueuePacket(Packet p)
        {
            if (QueuedPackets.Count < Capacity)
            {
                QueuedPackets.Add(p);

                OnNewPacketEnqued(p);
                p.OnEnqueuedToPort(this);

                if (QueuedPackets.Count == 1)
                {
                    // Initial packet.
                    OnNewPacketOnDeck(Peek());
                }

                PlayClip(PacketSourceSoundEffect.PacketEnqueued);
            }
        }

        public Packet DequeuePacket(int i = 0)
        {
            if (QueuedPackets.Count > i)
            {
                Packet popped = QueuedPackets[i];
                QueuedPackets.RemoveAt(i);

                if (QueuedPackets.Count == 0)
                {
                    OnEmptied();
                }
                OnDequeued(popped);

                // New packet is 'on deck'.
                if (i == 0 && !IsEmpty())
                {
                    OnNewPacketOnDeck(Peek());
                }

                Info.NumQueuedPackets--;
                Info.QueuedPackets = QueuedPackets;

                return popped;
            }

            // Indicates empty queue.
            return null;
        }

        public Packet Peek()
        {
            if (QueuedPackets.Count > 0)
            {
                return QueuedPackets[0];
            }

            return null;
        }

        public virtual void OnLinkStarted(Link l)
        {
            ActiveLinks.Add(l);

            if (!IsEmpty())
            {
                // Dequeue packet and load it onto link.
                var packet = DequeuePacket();
                l.EnqueuePacket(packet);

                packet.OnDequeuedFromPort(this, l);
            }

            // Listen for sever events.
            l.OnSever += (Link severed, SeverCause cause, float totalLength) =>
            {
                OnTransmissionSevered(cause, l);
            };

            l.OnTransmissionStarted += OnTransmissionStarted;

            if (OnPendingLinkStarted != null)
            {
                OnPendingLinkStarted.Invoke(l);
            }
        }

        public virtual void OnLinkEstablished(Link l, PacketSink t)
        {

        }

        public virtual void OnPacketWarning(Packet p)
        {

        }

        public virtual void OnPacketHasExpired(Packet p)
        {
            PlayClip(PacketSourceSoundEffect.PacketDropped);

            if (OnPacketExpired != null)
            {
                OnPacketExpired.Invoke(p);
            }

            GameManager.GetInstance().ReportPacketDropped(p);
        }

        protected virtual void OnEmptied()
        {
        }

        protected virtual void OnTransmissionStarted(Link l, Packet p)
        {

        }

        protected virtual void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            var index = ActiveLinks.FindIndex(
                link => link.GetInstanceID() == severedLink.GetInstanceID());
            if (index != -1)
            {
                ActiveLinks.RemoveAt(index);
            }
        }

        protected virtual void OnDequeued(Packet p)
        {
            if (OnPacketDequeued != null)
            {
                OnPacketDequeued.Invoke(p);
            }
        }

        protected virtual void OnNewPacketEnqued(Packet p)
        {
            Info.NumQueuedPackets++;
            Info.QueuedPackets = QueuedPackets;

            // Subscribe to packet expiration warning events.
            p.OnExpireWarning += OnPacketWarning;

            if (OnPacketEnqueued != null)
            {
                OnPacketEnqueued.Invoke(p);
            }
        }

        protected virtual void OnNewPacketOnDeck(Packet p)
        {
            p.OnDeckAtPort(this);
        }

        public SourceInfo portInfo
        {
            get
            {
                return new SourceInfo(
                    transform.position,
                    transform.rotation);
            }
        }
    }
}
