using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Port : MonoBehaviour
    {
        public Transform ConnectionPoint;
        public List<Packet> QueuedPackets;

        public void EnqueuePacket(Packet p)
        {
            QueuedPackets.Add(p);
            
            OnNewPacket(p);
        }

        public Packet DequeuePacket()
        {
            if (QueuedPackets.Count > 0)
            {
                Packet popped = QueuedPackets[0];
                QueuedPackets.RemoveAt(0);

                if (QueuedPackets.Count == 0)
                {
                    OnEmptied();
                }

                return popped;
            }

            // Indicates empty queue.
            return null;
        }

        private void OnEmptied()
        {
            // TODO
        }

        private void OnNewPacket(Packet p)
        {
            // TODO
        }

        private IEnumerator Flash()
        {
            return null;
        }

        private IEnumerator Wilt()
        {
            return null;
        }
    }
}
