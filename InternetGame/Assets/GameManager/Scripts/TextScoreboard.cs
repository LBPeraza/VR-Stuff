using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternetGame
{
    public class TextScoreboard : Scoreboard
    {
        public Text DroppedPacketsDisplay;
        public Text DeliveredPacketsDisplay;
        public Text TimeDisplay;

        public override void Initialize(GameScore score)
        {
            base.Initialize(score);

            if (DroppedPacketsDisplay == null)
            {
                DroppedPacketsDisplay = this.transform.Find("Canvas/DroppedPacketsDisplay").GetComponent<Text>();
            }

            if (DeliveredPacketsDisplay == null)
            {
                DeliveredPacketsDisplay = this.transform.Find("Canvas/DeliveredPacketsDisplay").GetComponent<Text>();
            }

            if (TimeDisplay == null)
            {
                TimeDisplay = this.transform.Find("Canvas/TimeDisplay").GetComponent<Text>();
            }

            UpdateScore(score);
        }

        public override void UpdateScore(GameScore score)
        {
            base.UpdateScore(score);

            Score = score;

            DroppedPacketsDisplay.text = score.PacketsDropped + "";
            DeliveredPacketsDisplay.text = score.PacketsDelivered + "";
            TimeDisplay.text = score.Time + "" ;
        }
    }
}

