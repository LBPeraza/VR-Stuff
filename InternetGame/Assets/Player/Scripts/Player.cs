using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Player : MonoBehaviour
    {
        public LinkController LeftCursor;
        public LinkController RightCursor;

        public float TotalBandwidth;
        public float MaxBandwidth;

        public void Initialize()
        {
            TotalBandwidth = MaxBandwidth;

            if (LeftCursor != null)
            {
                LeftCursor.Initialize(false /* is right hand */, this);
            }

            if (RightCursor != null)
            {
                RightCursor.Initialize(true /* is right hand */, this);
            }
        }
        
        public bool IsOutOfBandwidth()
        {
            return TotalBandwidth <= 0;
        }

    }
}

