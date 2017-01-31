using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class BandwidthDisplay : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void UpdateRemainingBandwidth(float percentage);
    }
}
