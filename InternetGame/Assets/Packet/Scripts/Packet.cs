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
        public int Size; // In "bytes".
        public Color Color;
        public Material Saturated;
        public Material Destaturated;
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

        protected float saturationPenalty = 0.0f;
        protected float valuePenalty = 0.5f;
        protected Color MakeLighter(Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            var newColor = Color.HSVToRGB(
                h, 
                Mathf.Clamp01(s - saturationPenalty), 
                Mathf.Clamp01(v - valuePenalty));
            return newColor;
        }

        public void SetSaturatedColor(Color c)
        {
            Saturated.SetColor("_EmissionColor", c);
            Saturated.color = c;
            Destaturated.color = MakeLighter(c);
        }

        public virtual void Initialize()
        {
            Color = (Color)PacketSpawner.AddressToColor[Destination];
        }

        public virtual void OnDeckAtPort(PacketSource p)
        {
            IsOnDeck = true;
            OnDeckTime = Time.fixedTime;
        }

        public virtual void OnEnqueuedToPort(PacketSource p)
        {
            EnqueuedTime = Time.fixedTime;

            Source = p;
        }

        public virtual void OnDequeuedFromPort(PacketSource p, Link l)
        {
            DequeuedTime = Time.fixedTime;
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
            // Don't put anything critical in here -- Virus overrides this without calling base.
            GameManager.ReportPacketDelivered(this);
        }

        public virtual void OnTransmissionStarted(Link l, Packet p)
        {
            TransmittingLink = l;
            l.OnTransmissionProgress += OnTransmissionProgress;
        }

        public virtual void OnTransmissionProgress(float percentageDone)
        {
            
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
            if (IsOnDeck)
            {
                if (!HasAlerted && Time.fixedTime > OnDeckTime + AlertTime)
                {
                    // Alert player to expiring packet.
                    ExpireWarning();

                    HasAlerted = true;
                }
                if (!HasDropped && Time.fixedTime > OnDeckTime + Patience)
                {
                    // Drop packet.
                    Expire();

                    HasDropped = true;
                    IsOnDeck = false;
                }
            }
        }
    }
}
