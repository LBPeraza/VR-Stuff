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

    public enum MethodOfEntry
    {
        Unset,
        Self,
        FallingPacketHopper
    }

    public enum PacketLoadingBehavior
    {
        Unset,
        PutPacketOnLinkWhenLinkStarted,
        PutPacketOnLinkWhenLinkEstablished
    }

    [Serializable]
    public class SourceInfo : PortInfo
    {
        public SourceInfo(Vector3 location, Quaternion orientation)
            : base(location, orientation)
        {
        }
    }

    public class PacketSource : MonoBehaviour, ResourceLoadable, PacketProcessor
    {
        [Header("Packet Spawning")]
        public MethodOfEntry MethodOfEntry;

        [Header("Indicator Settings")]
        [Tooltip("The indicator for this port. This takes priority over the prefab field.")]
        public PacketSourceIndicator Indicator;
        public PacketSourceIndicator IndicatorPrefab;

        [Header("Source Settings")]
        public string Address;
        public int Capacity { get; set; }
        public Transform LinkConnectionPoint;
        public PacketLoadingBehavior PacketLoadingBehavior;

        [HideInInspector]
        public GameObject PacketContainer;
        [HideInInspector]
        public List<Packet> QueuedPackets;
        [HideInInspector]
        public List<Link> ActiveLinks;
        [HideInInspector]
        public PacketSourceInfo Info;

        public event EventHandler<PacketEventArgs> OnPacketEnqueued;
        public event EventHandler<PacketEventArgs> OnPacketDequeued;
        public event EventHandler<LinkEventArgs> OnPendingLinkStarted;
        public event EventHandler<PacketEventArgs> OnPacketExpired;

        public event EventHandler<EstablishedLinkEventArgs> LinkEstablished;

        [HideInInspector]
        public PacketProcessor EntryPoint;

        protected Connector Connector;

        protected AudioSource EnqueuedAudioSource;
        protected AudioSource PacketDroppedAudioSource;
        protected AudioSource PacketWarningAudioSource;

        protected AudioClip PacketWarningClip;
        protected AudioClip PacketDroppedClip;
        protected AudioClip PacketEnqueuedClip;

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

            if (Indicator == null && IndicatorPrefab == null)
            {
                IndicatorPrefab = Resources.Load<PacketSourceIndicator>("Prefabs/RingIndicator");
            }

            Hexagon.LoadResources();
        }

        public virtual void Initialize()
        {
            LoadResources();

            InitializeAudio();

            ActiveLinks = new List<Link>();
            QueuedPackets = new List<Packet>();

            if (Capacity <= 0)
            {
                Capacity = 5;
            }

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

            if (PacketLoadingBehavior == PacketLoadingBehavior.Unset)
            {
                Debug.LogWarning("Packet Loading Behavior is not set -- defaulting to PacketSourceFactory setting.");
                PacketLoadingBehavior = PacketSourceFactory.PacketLoadingBehavior;
            }
            if (MethodOfEntry == MethodOfEntry.Unset)
            {
                Debug.LogWarning("Method of Entry is not set -- defaulting to PacketSourceFactory setting.");
                MethodOfEntry = PacketSourceFactory.PacketMethodOfEntry;
            }

            switch (MethodOfEntry)
            {
                case MethodOfEntry.Self:
                    EntryPoint = this;
                    break;
                case MethodOfEntry.FallingPacketHopper:
                    var packetHopper = new FallingPacketHopper();
                    packetHopper.Initialize(this, Indicator);
                    Indicator = null;
                    EntryPoint = packetHopper;
                    break;
            }

            if (Indicator != null)
            {
                Indicator.Initialize(this);
            }
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

        protected virtual void PutPacketOnLink(Link l)
        {
            if (!IsEmpty())
            {
                // Dequeue packet and load it onto link.
                var packet = DequeuePacket();
                l.EnqueuePacket(packet);

                packet.OnDequeuedFromPort(this, l);
            }
        }

        public virtual void OnLinkStarted(Link l)
        {
            ActiveLinks.Add(l);

            if (PacketLoadingBehavior == PacketLoadingBehavior.PutPacketOnLinkWhenLinkStarted)
            {
                PutPacketOnLink(l);
            }
            else
            {
                // If we don't move the packet to the link now, we still need
                // to inform the link of what sort of packet is running on it.
                l.SetPacket(Peek());
            }

            // Listen for sever events.
            l.OnSever += (Link severed, SeverCause cause, float totalLength) =>
            {
                OnTransmissionSevered(cause, l);
            };

            l.TransmissionStarted += OnTransmissionStarted;

            if (OnPendingLinkStarted != null)
            {
                OnPendingLinkStarted.Invoke(this, new LinkEventArgs { Link = l });
            }
        }

        public virtual void OnLinkEstablished(Link l, PacketSink t)
        {
            if (PacketLoadingBehavior == PacketLoadingBehavior.PutPacketOnLinkWhenLinkEstablished)
            {
                PutPacketOnLink(l);
            }

            if (LinkEstablished != null)
            {
                LinkEstablished.Invoke(this, new EstablishedLinkEventArgs
                {
                    Packet = l.Packet,
                    Source = this,
                    Sink = t
                });
            }
        }

        public virtual void OnPacketWarning(Packet p)
        {

        }

        public virtual void OnPacketHasExpired(Packet p)
        {
            PlayClip(PacketSourceSoundEffect.PacketDropped);

            if (OnPacketExpired != null)
            {
                OnPacketExpired.Invoke(this, new PacketEventArgs { Packet = p });
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
                OnPacketDequeued.Invoke(this, new PacketEventArgs { Packet = p });
            }
        }

        protected virtual void OnNewPacketEnqued(Packet p)
        {
            Info.NumQueuedPackets++;
            Info.QueuedPackets = QueuedPackets;

            // Subscribe to packet expiration warning events.
            p.ExpireWarning += OnPacketWarning;

            if (OnPacketEnqueued != null)
            {
                OnPacketEnqueued.Invoke(this, new PacketEventArgs { Packet = p });
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
