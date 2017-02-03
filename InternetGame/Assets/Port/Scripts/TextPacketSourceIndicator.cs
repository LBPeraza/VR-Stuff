using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace InternetGame
{
    public class TextPacketSourceIndicator : PacketSourceIndicator
    {
        public Text NumQueuedPacketsText;
        public Text CapacityText;

        public override void OnPacketDequeued(Packet p)
        {
            base.OnPacketDequeued(p);

            UpdateDisplay(Source.Info);
        }

        public override void OnPacketEnqueued(Packet p)
        {
            base.OnPacketEnqueued(p);

            UpdateDisplay(Source.Info);
        }

        public void UpdateDisplay(PacketSourceInfo info)
        {
            NumQueuedPacketsText.text = info.NumQueuedPackets + "";
            CapacityText.text = info.Capacity + "";
        }

    }
}

