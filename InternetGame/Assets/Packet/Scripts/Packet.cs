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
        public abstract void OnEnqueuedToPort(PacketSource p);
        public virtual void OnDequeuedFromPort(PacketSource p, Link l)
        {
            l.OnTransmissionProgress += OnTransmissionProgress;
        }
        public abstract void OnDequeuedFromLink(Link l, PacketSink p);
        public abstract void OnTransmissionProgress(float percentageDone);
    }
}
