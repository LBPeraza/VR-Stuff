﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FlashingPacketSink : PacketSink
    {
        IEnumerator flashingCoroutine;

        public float FlashRate; // Somewhere from (0, 2] is reasonable.
        public Color StartColor;
        public Color EndColor;

        public override void OnBecameOptionForLink(Link l)
        {
            base.OnBecameOptionForLink(l);

            StartFlashing();
        }

        public override void OnNoLongerOptionForLink(Link l)
        {
            base.OnNoLongerOptionForLink(l);

            EndFlashing();
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