using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSource : MonoBehaviour
    {
        public List<Packet> QueuedPackets;
        public Link ActiveLink;

        public void EnqueuePacket(Packet p)
        {
            QueuedPackets.Add(p);
            
            OnNewPacketEnqued(p);
        }

        public Packet DequeuePacket(int i = 0)
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

        public Packet Peek()
        {
            if (QueuedPackets.Count > 0)
            {
                return QueuedPackets[0];
            }

            return null;
        }

        private void FindAndSendPacketTo(Link l, PacketSink t)
        {
            Packet p = Peek();
            if (p.Destination == t.Address)
            {
                p.OnDequeuedFromPort(this, l);
                l.EnqueuePacket(DequeuePacket());
                OnTransmissionStarted(l);
            }   
        }

        public virtual void OnLinkStarted(Link l)
        {
            ActiveLink = l;

            // Listen for sever events.
            l.OnSever += (float totalLength) =>
            {
                OnTransmissionSevered(l);
            };
        }

        public void OnLinkEstablished(Link l, PacketSink t)
        {
            FindAndSendPacketTo(l, t);
        }

        protected virtual void OnEmptied()
        {
        }

        protected virtual void OnTransmissionStarted(Link l)
        {
        }

        protected virtual void OnTransmissionSevered(Link severedLink)
        {
            ActiveLink = null;
        }

        protected virtual void OnNewPacketEnqued(Packet p)
        {
        }
    }
}
