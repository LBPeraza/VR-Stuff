using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FlashingPacketSource : PacketSource
    {
        IEnumerator flashingCoroutine;

        public float FlashRate; // Somewhere from (0, 2] is reasonable.
        public Color StartColor;

        public bool IsFlashing;

        public override void OnLinkStarted(Link l)
        {
            base.OnLinkStarted(l);

            EndFlashing();
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

            if (this.ActiveLink == null)
            {
                StartFlashing((Color) PacketSpawner.AddressToColor[p.Destination]);
            }
        }

        private void StartFlashing(Color flashColor)
        {
            if (!IsFlashing)
            {
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
            GetComponent<Renderer>().material.color = StartColor;
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
