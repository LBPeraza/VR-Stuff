using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class GlowWithRoom : MonoBehaviour
    {
        public Material Material;
        public Color LitColor;
        public Color UnlitColor = Color.black;

        public AudioSource Audio;
        public int Low = 0;
        public int High = 100;
        public float MaxIntensity = 0.2f;
        public float IntensityThreshold = 0.7f;
        public float IntensityDropOff = 0.005f;

        public float Intensity = 0.0f;

        private Material materialCopy;
        private float lastSetIntensity;
        private float[] frequencyData;
        private int numSamples = 1024;
        private float frequencyMax;

        void Start()
        {
            // Make a copy of the material.
            materialCopy = new Material(Material);

            // Set the accents of all the children to this copy.
            foreach (Transform accent in transform)
            {
                accent.GetComponent<Renderer>().material = materialCopy;
            }

            SoundtrackMetadata metadata = SoundtrackUtility.GetMetadata(Audio.clip);
            Low = metadata.BeginLowBand;
            High = metadata.EndLowBand;
            MaxIntensity = metadata.MaxIntensity;

            frequencyData = new float[numSamples];
            frequencyMax = AudioSettings.outputSampleRate / 2;

            LitColor = Material.GetColor("_EmissionColor");
        }

        private float GetBandVol(AudioSource audio, float low, float high)
        {
            low = Mathf.Clamp(low, 20, frequencyMax);
            high = Mathf.Clamp(high, low, frequencyMax);

            audio.GetSpectrumData(frequencyData, 0, FFTWindow.BlackmanHarris);
            int n1 = (int)Mathf.Floor(low * numSamples / frequencyMax);
            int n2 = (int)Mathf.Floor(high * numSamples / frequencyMax);

            float sum = 0.0f;
            for (int i = n1; i <= n2; i++)
            {
                sum += frequencyData[i];
            }

            return sum / (n2 - n1 + 1);
        }

        /// <summary>
        /// Sets the intensity of the gameobject's glow.
        /// </summary>
        /// <param name="t">A scalar from 0 to 1.</param>
        private void SetGlowIntensity(float t)
        {
            if (t < IntensityThreshold)
            {
                t = Mathf.Clamp01(lastSetIntensity - IntensityDropOff);
            }
            materialCopy.SetColor("_EmissionColor", Color.Lerp(UnlitColor, LitColor, t));

            lastSetIntensity = t;
        }

        // Update is called once per frame
        void Update()
        {
            Intensity = GetBandVol(Audio, Low, High);
            float volumeScalar = Audio.volume;
            float scaledIntensity = Intensity / (MaxIntensity * volumeScalar);
            SetGlowIntensity(scaledIntensity);
        }
    }
}

