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

    public class PacketSource : MonoBehaviour
	{
		public PortInfo info;

        public List<Packet> QueuedPackets;
        public List<Link> ActiveLinks;
        public PacketSourceIndicator Indicator;
        public int Capacity = 5;
        public PacketSourceInfo Info;

        public AudioSource AudioSource;
        public AudioClip PacketWarningClip;
        public AudioClip PacketDroppedClip;
        public AudioClip PacketEnqueuedClip;

        public bool HasUnfinishedLink()
        {
            return ActiveLinks.Exists(link => !link.Finished);
        }

        public void Initialize()
        {
            ActiveLinks = new List<Link>();
            QueuedPackets = new List<Packet>();

            Info.Capacity = Capacity;
            Info.QueuedPackets = QueuedPackets;
            Info.NumQueuedPackets = 0;

            this.info = new PortInfo(
                this.transform.position,
                this.transform.rotation
            );

            if (Indicator == null)
            {
                var prefab = Resources.Load<GameObject>("RingIndicator");
                var indicator = Instantiate(prefab, this.transform, false);

                Indicator = indicator.GetComponent<PacketSourceIndicator>();
            }

            LoadAudioClips();

            Indicator.Initialize(Info);
        }

        public void LoadAudioClips()
        {
            if (AudioSource == null)
            {
                AudioSource = GetComponent<AudioSource>();
            }

            if (PacketWarningClip == null)
            {
                PacketWarningClip = Resources.Load<AudioClip>("packet_alert");
            }

            if (PacketDroppedClip == null)
            {
                PacketDroppedClip = Resources.Load<AudioClip>("packet_dropped");
            }

            if (PacketEnqueuedClip == null)
            {
                PacketEnqueuedClip = Resources.Load<AudioClip>("packet_enqueued");
            }
        }

        public void PlayClip(PacketSourceSoundEffect effect)
        {
            float volume = 0.5f;
            AudioClip clip = PacketEnqueuedClip;
            bool repeat = false;

            switch (effect)
            {
                case PacketSourceSoundEffect.PacketDropped:
                    clip = PacketDroppedClip;
                    volume = 1.0f;
                    break;
                case PacketSourceSoundEffect.PacketEnqueued:
                    clip = PacketEnqueuedClip;
                    volume = .75f;
                    break;
                case PacketSourceSoundEffect.PacketWarning:
                    clip = PacketWarningClip;
                    volume = 1.0f;
                    break;
            }

            AudioSource.Stop();
            AudioSource.clip = clip;
            AudioSource.volume = volume;
            AudioSource.loop = repeat;
            AudioSource.Play();
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

                Info.NumQueuedPackets--;
                Info.QueuedPackets = QueuedPackets;

                Indicator.UpdatePacketSourceInfo(Info);

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

        private void FindAndSendPacketTo(Link l, PacketSink t)
        {
            Packet p = Peek();
            if (p != null && p.Destination == t.Address)
            {
                var packet = DequeuePacket();
                l.EnqueuePacket(packet);
                packet.OnDequeuedFromPort(this, l);
                OnTransmissionStarted(l);
            }   
        }

        public virtual void OnLinkStarted(Link l)
        {
            ActiveLinks.Add(l);

            // Notify the next packet that it is staged for a link.
            Peek().OnFutureLinkStarted(l);

            // Listen for sever events.
            l.OnSever += (Link severed, SeverCause cause, float totalLength) =>
            {
                OnTransmissionSevered(cause, l);
            };
        }

        public virtual void OnLinkEstablished(Link l, PacketSink t)
        {
            FindAndSendPacketTo(l, t);
        }

        public virtual void OnPacketDropped(Packet p)
        {
            PlayClip(PacketSourceSoundEffect.PacketDropped);
        }

        protected virtual void OnEmptied()
        {
        }

        protected virtual void OnTransmissionStarted(Link l)
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

        }

        protected virtual void OnNewPacketEnqued(Packet p)
        {
            Info.NumQueuedPackets++;
            Info.QueuedPackets = QueuedPackets;

            Indicator.UpdatePacketSourceInfo(Info);
        }

        
    }
}
