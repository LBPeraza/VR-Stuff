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
        public Cursor LeftCursor;
        public Cursor RightCursor;

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
                LeftCursor.Initialize(this, false /* is right hand */);
            }

            if (RightCursor != null)
            {
                RightCursor.Initialize(this, true /* is right hand */);
            }
        }

        public void Update()
        {
            CurrentState.BandwidthRemaining = TotalBandwidth;
            UpdatePlayerUI();
        }

        public void UpdatePlayerUI()
        {
            if (PlayerUI != null)
            {
                PlayerUI.UpdatePlayerState(CurrentState);
            }
        }
        
        public bool IsOutOfBandwidth()
        {
            return TotalBandwidth <= 0;
        }

    }
}

