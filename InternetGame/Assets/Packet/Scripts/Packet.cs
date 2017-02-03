using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
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

        protected bool HasAlerted = false;
        protected bool HasDropped = false;

        protected float saturationPenalty = 0.2f;
        protected float lighterTransparency = 0.5f;
        protected Color MakeLighter(Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            var newColor = Color.HSVToRGB(h, Mathf.Clamp01(s - saturationPenalty), v);
            newColor.a = lighterTransparency;
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

            l.OnTransmissionStarted += OnTransmissionStarted;
        }
        public virtual void OnDequeuedFromLink(Link l, PacketSink p)
        {
            // Don't put anything critical in here -- Virus overrides this without calling base.
            GameManager.ReportPacketDelivered(this);
        }

        public virtual void OnTransmissionStarted(Link l, Packet p)
        {
            l.OnTransmissionProgress += OnTransmissionProgress;
        }

        public virtual void OnTransmissionProgress(float percentageDone)
        {

        }

        public virtual void ExpireWarning()
        {
            if (OnExpireWarning != null)
            {
                OnExpireWarning.Invoke(this);
            }

            Source.PlayClip(PacketSourceSoundEffect.PacketWarning);
        }

        public virtual void Expire()
        {
            // Dequeue packet.
            Source.DequeuePacket(Source.QueuedPackets.FindIndex(
                packet => packet.GetInstanceID() == this.GetInstanceID()));

            Source.OnPacketHasExpired(this);

            Destroy(this.gameObject);
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
