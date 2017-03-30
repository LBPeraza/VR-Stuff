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
        public Cursor PrimaryCursor;

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
                LeftCursor.Initialize(this, false /* is right hand */, false /* is primary hand */);
                PrimaryCursor = LeftCursor;
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
                RightCursor.Initialize(this, true /* is right hand */, false /* is primary hand */);
                PrimaryCursor = RightCursor;
            }

            if (PrimaryCursor == null)
            {
                Debug.LogError("PrimaryCursor property of LinkController is unset.");
            }
            else
            {
                SetPrimaryCursor(PrimaryCursor);
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

        public void SetPrimaryCursor(Cursor c)
        {
            // Set the old cursor to not be primary.
            PrimaryCursor.IsPrimary = false;

            // Replace old cursor with new one.
            PrimaryCursor = c;
            PrimaryCursor.IsPrimary = true;
            LinkController.GetInstance().Cursor = c;
        }

    }
}

