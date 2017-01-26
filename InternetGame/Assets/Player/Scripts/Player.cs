using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public struct PlayerState
    {
        public float BandwidthRemaining;
        public float MaximumBandwidth;
    }

    public class Player : MonoBehaviour
    {
        public LinkController LeftCursor;
        public LinkController RightCursor;

        public float TotalBandwidth;
        public float MaxBandwidth;

        public PlayerUI PlayerUI;

        public PlayerState CurrentState;

        public void Initialize()
        {
            TotalBandwidth = MaxBandwidth;

            CurrentState = new PlayerState();
            CurrentState.MaximumBandwidth = MaxBandwidth;
            CurrentState.BandwidthRemaining = TotalBandwidth;

            if (LeftCursor != null)
            {
                LeftCursor.Initialize(false /* is right hand */, this);
            }

            if (RightCursor != null)
            {
                RightCursor.Initialize(true /* is right hand */, this);
            }
        }

        public void Update()
        {
            CurrentState.BandwidthRemaining = TotalBandwidth;
            UpdatePlayerUI();
        }

        public void UpdatePlayerUI()
        {
            PlayerUI.UpdatePlayerState(CurrentState);
        }
        
        public bool IsOutOfBandwidth()
        {
            return TotalBandwidth <= 0;
        }

    }
}

