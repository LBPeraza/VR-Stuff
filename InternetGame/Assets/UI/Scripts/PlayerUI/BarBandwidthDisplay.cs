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
            UpdateRemainingBandwidth(100.0f);
        }

        public override void UpdateRemainingBandwidth(float percentage)
        {
            Text textDisplay = this.GetComponent<Text>();
            textDisplay.text = Output.Substring(0, (int) (percentage * MAX_NUM_BARS));
        }
    }
}
