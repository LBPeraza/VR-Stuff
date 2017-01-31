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
        public Color EndColor;

        public bool IsFlashing;

        public override void OnLinkStarted(Link l)
        {
            base.OnLinkStarted(l);

            EndFlashing();
        }

        protected override void OnTransmissionSevered(Link severedLink)
        {
            base.OnTransmissionSevered(severedLink);

            if (this.QueuedPackets.Count > 0)
            {
                StartFlashing();
            }
        }

        protected override void OnNewPacketEnqued(Packet p)
        {
            base.OnNewPacketEnqued(p);

            if (this.ActiveLink == null)
            {
                StartFlashing();
            }
        }

        private void StartFlashing()
        {
            if (!IsFlashing)
            {
                flashingCoroutine = Flash();
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

        private IEnumerator Flash()
        {
            while (true)
            {
                Color lerpedColor = Color.Lerp(StartColor, EndColor, Mathf.PingPong(Time.fixedTime * FlashRate, 1));
                GetComponent<Renderer>().material.color = lerpedColor;

                yield return null;
            } 
        }
    }

}
