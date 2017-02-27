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
        public Cursor LinkCursor;

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

            if (LeftCursor == null)
            {
                var leftCursorContainer = transform.Find("LeftCursor");
                if (leftCursorContainer)
                {
                    LeftCursor = leftCursorContainer.GetComponent<Cursor>();
                }
            }
            if (LeftCursor != null)
            {
                LeftCursor.Initialize(this, false /* is right hand */);
                LinkCursor = LeftCursor;
            }

            if (RightCursor == null)
            {
                var rightCursorContainer = transform.Find("RightCursor");
                if (rightCursorContainer)
                {
                    RightCursor = rightCursorContainer.GetComponent<Cursor>();
                }
            }
            if (RightCursor != null)
            {
                RightCursor.Initialize(this, true /* is right hand */);
                LinkCursor = RightCursor;
            }

            if (LinkCursor == null)
            {
                Debug.LogError("LinkCursor property of LinkController is unset.");
            }

            if (PlayerUI != null)
            {
                PlayerUI.Initialize(this);
            }

            // Clear out any residual instance, and initialize a new one.
            LinkController.ResetInstance();
            LinkController.GetInstance().Initialize(this);
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

