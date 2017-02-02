using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FlashingPacketSource : PacketSource
    {
        IEnumerator flashingCoroutine;

        public float FlashRate; // Somewhere from (0, 2] is reasonable.
        public Color NeutralColor;
        public Color StartColor;

        public bool IsFlashing;

        private float saturationPenalty = 0.7f;
        private Color MakeLighter(Color c)
        {
            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);

            return Color.HSVToRGB(h, (s - saturationPenalty), v);
        }

        public void Start()
        {
            NeutralColor = GetComponent<Renderer>().material.color;
        }

        public override void OnLinkStarted(Link l)
        {
            base.OnLinkStarted(l);

            EndFlashing();
        }

        public override void OnLinkEstablished(Link l, PacketSink t)
        {
            base.OnLinkEstablished(l, t);

            if (this.QueuedPackets.Count > 0)
            {
                StartFlashing((Color)PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (this.QueuedPackets.Count > 0)
            {
                StartFlashing((Color) PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }

        protected override void OnNewPacketEnqued(Packet p)
        {
            base.OnNewPacketEnqued(p);

            if (!HasUnfinishedLink())
            {
                StartFlashing((Color) PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }

        private void StartFlashing(Color flashColor)
        {
            if (!IsFlashing)
            {
                StartColor = MakeLighter(flashColor);
                flashingCoroutine = Flash(flashColor);
                StartCoroutine(flashingCoroutine);

                IsFlashing = true;
            }
        }

        private void EndFlashing()
        {
            if (IsFlashing)
            {
                StopCoroutine(flashingCoroutine);
                ResetColor();

                IsFlashing = false;
            }
        }

        private void ResetColor()
        {
            GetComponent<Renderer>().material.color = NeutralColor;
        }

        private IEnumerator Flash(Color flashColor)
        {
            while (true)
            {
                Color lerpedColor = Color.Lerp(StartColor, flashColor, Mathf.PingPong(Time.fixedTime * FlashRate, 1));
                GetComponent<Renderer>().material.color = lerpedColor;

                yield return null;
            } 
        }
    }

}
