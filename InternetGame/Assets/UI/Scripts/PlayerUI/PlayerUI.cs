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

            if (p.LeftCursor.Input != null)
            {
                SetUpInputHandlers(p.LeftCursor.Input);
            }
            else
            {
                p.LeftCursor.OnControllerFound += SetUpInputHandlers;
            }

            if (p.RightCursor.Input != null)
            {
                SetUpInputHandlers(p.RightCursor.Input);
            }
            else
            {
                p.RightCursor.OnControllerFound += SetUpInputHandlers;
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
            float remainingPercentage = (float)state.BandwidthRemaining / state.MaximumBandwidth;
            BandwidthDisplay.UpdateRemainingBandwidth(remainingPercentage);
        }
    }
}
