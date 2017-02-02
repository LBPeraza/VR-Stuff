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
        public float EnqueuedTime;
        public float Patience;
        public float AlertTime;

        public PacketSource Source;

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
        public virtual void OnEnqueuedToPort(PacketSource p)
        {
            Source = p;

            EnqueuedTime = Time.fixedTime;
            IsWaitingAtPort = true;
        }
        public virtual void OnDequeuedFromPort(PacketSource p, Link l)
        {
            l.OnTransmissionProgress += OnTransmissionProgress;
        }
        public virtual void OnDequeuedFromLink(Link l, PacketSink p)
        {

        }
        public virtual void OnTransmissionProgress(float percentageDone)
        {

        }

        public virtual void Alert()
        {

        }

        public virtual void Drop()
        {
            // Dequeue packet.
            Source.DequeuePacket(Source.QueuedPackets.FindIndex(
                packet => packet.GetInstanceID() == this.GetInstanceID()));

            GameManager.ReportPacketDropped(this);

            Destroy(this.gameObject);
        }

        public virtual void Update()
        {
            if (IsWaitingAtPort)
            {
                if (!HasAlerted && Time.fixedTime > EnqueuedTime + AlertTime)
                {
                    // Alert player to expiring packet.
                    Alert();

                    HasAlerted = true;
                }
                if (!HasDropped && Time.fixedTime > EnqueuedTime + Patience)
                {
                    // Drop packet.
                    Drop();

                    HasDropped = true;
                    IsWaitingAtPort = false;
                }
            }
        }
    }
}
