using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternetGame
{ 
    public class BarBandwidthDisplay : BandwidthDisplay
    {
        public int MAX_NUM_BARS = 50;
        public string Output = "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll";

        public override void Initialize()
        {
            base.Initialize();

            UpdateRemainingBandwidth(1.0f);
        }

        public override void LoadResources()
        {
            
        }

        public override void UpdateRemainingBandwidth(float percentage)
        {
            if (percentage <= 100.0f && percentage >= 0.0f)
            {
                Text textDisplay = this.GetComponent<Text>();
                textDisplay.text = Output.Substring(0, (int)(percentage * MAX_NUM_BARS));
            }
        }
    }
}
