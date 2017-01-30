using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSource : MonoBehaviour
    {
        public List<Packet> QueuedPackets;

        public void EnqueuePacket(Packet p)
        {
            QueuedPackets.Add(p);
            
            OnNewPacket(p);
        }

        public Packet DequeuePacket(int i)
        {
            if (QueuedPackets.Count > i)
            {
                Packet popped = QueuedPackets[i];
                QueuedPackets.RemoveAt(i);

                if (QueuedPackets.Count == 0)
                {
                    OnEmptied();
                }

                return popped;
            }

            // Indicates empty queue.
            return null;
        }

        private void FindAndSendPacketTo(Link l, PacketSink t)
        {
            for (int i = 0; i < QueuedPackets.Count; i++)
            {
                Packet p = QueuedPackets[i];
                if (p.Destination == t.Address)
                {
                    l.EnqueuePacket(DequeuePacket(i));
                }
            }
        }

        public void OnLinkEstablished(Link l, PacketSink t)
        {
            FindAndSendPacketTo(l, t);
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
