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

        public override void UpdatePacketSourceInfo(PacketSourceInfo info)
        {
            NumQueuedPacketsText.text = info.NumQueuedPackets + "";
            CapacityText.text = info.Capacity + "";
        }

    }
}

