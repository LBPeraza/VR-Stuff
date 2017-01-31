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

        public override void OnLinkStarted(Link l)
        {
            base.OnLinkStarted(l);

            Debug.Log("Link started!");

            EndFlashing();
        }

        protected override void OnTransmissionSevered(Link severedLink)
        {
            base.OnTransmissionSevered(severedLink);

            Debug.Log("Transmission severed and " + this.QueuedPackets.Count + " packets left");

            if (this.QueuedPackets.Count > 0)
            {
                StartFlashing();
            }
        }

        protected override void OnNewPacketEnqued(Packet p)
        {
            if (this.ActiveLink == null)
            {
                StartFlashing();
            }
        }

        private void StartFlashing()
        {
            flashingCoroutine = Flash();
            StartCoroutine(flashingCoroutine);
        }

        private void EndFlashing()
        {
            StopCoroutine(flashingCoroutine);
            ResetColor();
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
