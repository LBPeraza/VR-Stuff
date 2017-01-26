using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{ 
    public class BarBandwidthDisplay : MonoBehaviour, BandwidthDisplay
    {
        public int MAX_NUM_BARS = 50;
        public string Output = "llllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll";

        public void Initialize()
        {
            UpdateRemainingBandwidth(100.0f);
        }

        public void UpdateRemainingBandwidth(float percentage)
        {
            GUIText textDisplay = this.GetComponent<GUIText>();
            textDisplay.text = Output.Substring(0, (int)percentage * MAX_NUM_BARS);
        }
    }
}
