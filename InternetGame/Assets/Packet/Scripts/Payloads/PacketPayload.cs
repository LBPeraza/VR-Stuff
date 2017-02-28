using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketPayload : MonoBehaviour
    {
        public int Size; // In "bytes".

        public Color Color;

        public Material Saturated;
        public Material Destaturated;

        public virtual void Initialize(Color c)
        {
            Color = c;
        }

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

        public virtual void OnTransmissionStarted(Link l, Packet p)
        {
            l.OnSever += OnSever;
        }

        public virtual void OnTransmissionProgress(float percentage)
        {

        }

        public virtual void OnSever(Link severerd, SeverCause cause, float totalLength)
        {

        }

        public virtual void OnDequeuedFromLink(Link l, PacketSink p)
        {

        }
    }
}

