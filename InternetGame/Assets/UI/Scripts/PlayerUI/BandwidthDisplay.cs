using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public interface BandwidthDisplay 
    {
        void Initialize();
        void UpdateRemainingBandwidth(float percentage);
    }
}
