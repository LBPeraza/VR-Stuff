using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum PacketDroppedCause
    {
        EarlyTermination,
        Severed,
        Expired
    }

    public abstract class Packet : MonoBehaviour
    {
        public PacketPayload Payload;
        public Color Color;
        public string Destination;

        public bool IsWaitingAtPort;
        public bool IsOnDeck;
        public float OnDeckTime;
        public float EnqueuedTime;
        public float DequeuedTime;
        public float Patience;
        public float AlertTime;

        public PacketSource Source;

        public delegate void OnExpireWarningHandler(Packet p);
        public OnExpireWarningHandler OnExpireWarning;

        public delegate void OnSavedHandler(Packet p);
        public OnSavedHandler OnSaved;

        protected bool HasAlerted = false;
        protected bool HasDropped = false;

        protected Link TransmittingLink;
        
        public virtual void Initialize()
        {
            Color = (Color)PacketSpawner.AddressToColor[Destination];

            Payload.Initialize(Color);
        }

        public virtual void OnDeckAtPort(PacketSource p)
        {
            IsOnDeck = true;
            OnDeckTime = GameManager.GetInstance().GameTime();
        }

        public virtual void OnEnqueuedToPort(PacketSource p)
        {
            EnqueuedTime = GameManager.GetInstance().GameTime();

            Source = p;
        }

        public virtual void OnDequeuedFromPort(PacketSource p, Link l)
        {
            DequeuedTime = GameManager.GetInstance().GameTime();
            IsWaitingAtPort = false;
            IsOnDeck = false;

            if (HasAlerted && OnSaved != null)
            {
                OnSaved.Invoke(this);
            }

            l.OnTransmissionStarted += OnTransmissionStarted;
        }

        public virtual void OnDequeuedFromLink(Link l, PacketSink p)
        {
            Payload.OnDequeuedFromLink(l, p);

            // Don't put anything critical in here -- Virus overrides this without calling base.
            GameManager.GetInstance().ReportPacketDelivered(this);
        }

        public virtual void OnTransmissionStarted(Link l, Packet p)
        {
            Payload.OnTransmissionStarted(l, p);

            TransmittingLink = l;
            l.OnTransmissionProgress += OnTransmissionProgress;
        }

        public virtual void OnTransmissionProgress(float percentageDone)
        {
            Payload.OnTransmissionProgress(percentageDone);
        }

        public virtual void OnDropped(PacketDroppedCause cause)
        {
            Destroy(this.gameObject);
        }

        protected virtual void ExpireWarning()
        {
            if (OnExpireWarning != null)
            {
                OnExpireWarning.Invoke(this);
            }

            Source.PlayClip(PacketSourceSoundEffect.PacketWarning);
        }

        protected virtual void Expire()
        {
            // Dequeue packet.
            Source.DequeuePacket(Source.QueuedPackets.FindIndex(
                packet => packet.GetInstanceID() == this.GetInstanceID()));

            Source.OnPacketHasExpired(this);

            this.OnDropped(PacketDroppedCause.Expired);
        }

        public virtual void Update()
        {
            
        }
    }
}
