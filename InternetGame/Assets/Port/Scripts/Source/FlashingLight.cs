﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class FlashingLight : MonoBehaviour
    {
        [Header("Light Settings")]
        public Light WarningLight;
        public GameObject WarningBulb;
        public float WarningLightMaxIntensity = 5.0f;
        public float WarningLightMinIntensity = 1.0f;
        public Color WarningLightColor = Color.red;
        public float FlashRate = 1.0f;

        protected Animation Animation;
        protected AnimationClip Rainbow;

        protected bool IsFlashing = false;
        Coroutine flashingAnimation;

        public void LoadResources()
        {
            Rainbow = Resources.Load<AnimationClip>("Animations/RainbowLight");
        }

        public void Initialize()
        {
            LoadResources();

            Animation = GetComponent<Animation>();

            if (WarningBulb != null)
            {
                Material copy = new Material(WarningBulb.GetComponent<Renderer>().material);
                WarningBulb.GetComponent<Renderer>().material = copy;
            }

            if (Animation != null)
            {
                Animation.Stop();
            }

            SetBulb(WarningLightColor, 0.0f);
        }

        public void FlashRainbow()
        {
            Animation.clip = Rainbow;
            Animation.wrapMode = WrapMode.Loop;
            Animation.Play();
        }

        private void SetBulb(Color c, float intensity)
        {
            WarningLight.intensity = intensity;
            WarningBulb.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
        }

        private IEnumerator Flash(Color c)
        {
            while (true)
            {
                float t = 0.5f * Mathf.Cos(Time.fixedTime * FlashRate) + 0.5f;
                Color toSet = Color.Lerp(Color.black, c, t);
                float intensity = WarningLightMinIntensity + (t * (WarningLightMaxIntensity - WarningLightMinIntensity));

                SetBulb(toSet, intensity);

                yield return null;
            }
        }

        public void SetFlashing(bool flashing)
        {
            if (flashing != IsFlashing)
            {
                if (flashing)
                {
                    flashingAnimation = StartCoroutine(Flash(WarningLightColor));

                    IsFlashing = true;
                }
                else
                {
                    StopCoroutine(flashingAnimation);
                    SetBulb(Color.black, 0.0f);

                    IsFlashing = false;
                }
            }
        }
    }
}

