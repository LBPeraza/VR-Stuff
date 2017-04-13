using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternetGame
{
    public class TextWaveDisplay : WaveDisplay
    {
        public Text WaveDisplay;

        protected Animation Animation;
        protected AnimationClip FlashText;

        public virtual void LoadResources()
        {
            FlashText = Resources.Load<AnimationClip>("Animations/FlashText");
        }

        public override void Initialize(WaveLevelController waveLevelController)
        {
            base.Initialize(waveLevelController);

            LoadResources();

            Animation = gameObject.AddComponent<Animation>();
            Animation.AddClip(FlashText, "flash_text");

            if (WaveDisplay == null)
            {
                WaveDisplay = gameObject.GetComponentInChildren<Text>();
            }

            UpdateDisplay(1);
        }

        private void FlashDisplay()
        {
            Animation.Stop();
            Animation.wrapMode = WrapMode.Once;
            Animation.Play("flash_text");
        }

        private void UpdateDisplay(int waveNumber)
        {
            WaveDisplay.text = waveNumber.ToString();
        }

        protected override void OnWaveCleared(int currentWave)
        {
            UpdateDisplay(currentWave);
            FlashDisplay();
        }
    }
}

