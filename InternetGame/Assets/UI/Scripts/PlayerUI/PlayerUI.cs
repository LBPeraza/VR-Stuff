using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PlayerUI : MonoBehaviour
    {
        public BandwidthDisplay BandwidthDisplay;
        public PauseDisplay PauseDisplay;

        protected Player Player;

        public void Initialize(Player p)
        {
            Player = p;
            p.PrimaryCursorChanged += PrimaryCursorChanged;

            if (p.LeftCursor.Input != null)
            {
                SetUpInputHandlers(p.LeftCursor.Input);
            }
            else
            {
                p.LeftCursor.ControllerFound += SetUpInputHandlers;
            }

            if (p.RightCursor.Input != null)
            {
                SetUpInputHandlers(p.RightCursor.Input);
            }
            else
            {
                p.RightCursor.ControllerFound += SetUpInputHandlers;
            }

            if (BandwidthDisplay != null)
            {
                BandwidthDisplay.Initialize();
            }

            if (PauseDisplay != null)
            {
                PauseDisplay.Initialize();
            }
        }

        private void PrimaryCursorChanged(Cursor primary, Cursor secondary)
        {
            BandwidthDisplay.transform.SetParent(secondary.transform, false /* maintain world position */); 
        }

        protected void SetUpInputHandlers(VRTK.VRTK_ControllerEvents input)
        {
            input.AliasMenuOff += MenuToggle;
        }

        private void MenuToggle(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            GameManager.GetInstance().TogglePause();

            if (PauseDisplay != null)
            {
                if (GameManager.GetInstance().IsPaused)
                {
                    PauseDisplay.ShowPause();
                }
                else
                {
                    PauseDisplay.ShowUnpause();
                }
            }
        }

        public void UpdatePlayerState(PlayerState state)
        {
            if (BandwidthDisplay != null)
            {
                float remainingPercentage = (float)state.BandwidthRemaining / state.MaximumBandwidth;
                BandwidthDisplay.UpdateRemainingBandwidth(remainingPercentage);
            }
        }
    }
}
