using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PlayerUI : MonoBehaviour
    {
        public BandwidthDisplay BandwidthDisplay;

        public void UpdatePlayerState(PlayerState state)
        {
            float remainingPercentage = (float)state.BandwidthRemaining / state.MaximumBandwidth;
            BandwidthDisplay.UpdateRemainingBandwidth(remainingPercentage);
        }
    }
}
