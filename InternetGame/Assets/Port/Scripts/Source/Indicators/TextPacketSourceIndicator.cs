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

        protected int numQueued = 0;

        public override void OnPacketDequeued(object sender, PacketEventArgs p)
        {
            base.OnPacketDequeued(sender, p);

            numQueued += 1;

            UpdateDisplay(Processor.Capacity, numQueued);
        }

        public override void OnPacketEnqueued(object sender, PacketEventArgs p)
        {
            base.OnPacketEnqueued(sender, p);

            numQueued -= 1;

            UpdateDisplay(Processor.Capacity, numQueued);
        }

        public void UpdateDisplay(int capacity, int numQueued)
        {
            NumQueuedPacketsText.text = numQueued + "";
            CapacityText.text = capacity + "";
        }

    }
}

