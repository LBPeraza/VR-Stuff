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

    public enum PacketState
    {
        Unset,
        WaitingAtPort,
        OnDeck,
        WaitingOnLink,
        Transmitting,
        Transmitted,
        Expired
    }

    public abstract class Packet : MonoBehaviour
    {
        public PacketPayload Payload;
        public Color Color;
        public string Destination;

        public PacketState State;
        public float OnDeckTime;
        public float EnqueuedTime;
        public float DequeuedTime;
        public float Patience;
        public float AlertTime;

        public PacketSource Source;

        public delegate void OnExpireWarningHandler(Packet p);
        public OnExpireWarningHandler ExpireWarning;

        public delegate void OnSavedHandler(Packet p);
        public OnSavedHandler Saved;

        public delegate void OnDroppedHandler(Packet p);
        public OnDroppedHandler Dropped;

        public delegate void OnTransmittedHandler(Packet p);
        public OnTransmittedHandler Transmitted;

        public delegate void OnDestroyedHandler(Packet p);
        public OnDestroyedHandler Destroyed;

        protected bool HasAlerted = false;
        protected bool HasDropped = false;

        protected Link TransmittingLink;
        
        public virtual void Initialize()
        {
            State = PacketState.Unset;
            Color = (Color)GameUtils.AddressToColor[Destination];

            Payload.Initialize(Color);
        }

        public virtual void OnDeckAtPort(PacketSource p)
        {
            State = PacketState.OnDeck;
            OnDeckTime = GameManager.GetInstance().GameTime();
        }

        public virtual void OnEnqueuedToPort(PacketSource p)
        {
            State = PacketState.WaitingAtPort;
            EnqueuedTime = GameManager.GetInstance().GameTime();

            Source = p;
        }

        public virtual void OnDequeuedFromPort(PacketSource p, Link l)
        {
            DequeuedTime = GameManager.GetInstance().GameTime();

            State = PacketState.WaitingOnLink;

            if (HasAlerted && Saved != null)
            {
                Saved.Invoke(this);
            }

            l.TransmissionStarted += OnTransmissionStarted;
        }

        public virtual void OnDequeuedFromLink(Link l, PacketSink p)
        {
            State = PacketState.Transmitted;

            Payload.OnDequeuedFromLink(l, p);

            // Don't put anything critical in here -- Virus overrides this without calling base.
            GameManager.GetInstance().ReportPacketDelivered(this);
        }

        public virtual void OnTransmissionStarted(Link l, Packet p)
        {
            State = PacketState.Transmitting;

            Payload.OnTransmissionStarted(l, p);

            TransmittingLink = l;
            l.TransmissionProgressed += OnTransmissionProgress;
            l.TransmissionCompleted += (_ => OnTransmissionComplete());
        }

        public virtual void OnTransmissionProgress(float percentageDone)
        {
            Payload.OnTransmissionProgress(percentageDone);
        }

        public virtual void OnTransmissionComplete()
        {
            if (Transmitted != null)
            {
                Transmitted.Invoke(this);
            }

            if (Destroyed != null)
            {
                Destroyed.Invoke(this);
            }
        }

        public virtual void OnDropped(PacketDroppedCause cause)
        {
            if (Dropped != null)
            {
                Dropped.Invoke(this);
            }

            if (Destroyed != null)
            {
                Destroyed.Invoke(this);
            }

            Destroy(this.gameObject);
        }

        protected virtual void OnExpireWarning()
        {
            if (ExpireWarning != null)
            {
                ExpireWarning.Invoke(this);
            }

            Source.PlayClip(PacketSourceSoundEffect.PacketWarning);
        }

        protected virtual void Expire()
        {
            State = PacketState.Expired;

            if (Source != null)
            {
                var indexOfSelf = Source.QueuedPackets.FindIndex(
                    packet => packet.GetInstanceID() == this.GetInstanceID());

                if (indexOfSelf >= 0)
                {
                    // Dequeue packet.
                    Source.DequeuePacket();
                }

                Source.OnPacketHasExpired(this);
            }

            this.OnDropped(PacketDroppedCause.Expired);
        }

        public virtual void Update()
        {
            
        }
    }
}
