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

        public override void Initialize()
        {
            base.Initialize();

            NeutralColor = GetComponent<Renderer>().material.color;
        }

        protected override void OnNewPacketOnDeck(Packet p)
        {
            base.OnNewPacketOnDeck(p);

            if (!HasUnfinishedLink())
            {
                StartFlashing((Color)PacketSpawner.AddressToColor[p.Destination]);
            }
        }

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (cause == SeverCause.UnfinishedLink)
            {
                // When the player doesnt finish a link, we need to stop flashing.
                EndFlashing();
            }
        }

        protected override void OnTransmissionStarted(Link l, Packet p)
        {
            base.OnTransmissionStarted(l, p);

            if (IsEmpty())
            {
                EndFlashing();
            } else
            {
                StartFlashing((Color)PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }

        private void StartFlashing(Color flashColor)
        {
            if (IsFlashing)
            {
                EndFlashing();
            }

            StartColor = MakeLighter(flashColor);
            flashingCoroutine = Flash(flashColor);
            StartCoroutine(flashingCoroutine);

            IsFlashing = true;
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
