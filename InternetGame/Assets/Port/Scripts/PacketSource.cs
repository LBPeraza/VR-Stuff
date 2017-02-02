using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSource : MonoBehaviour
	{
		public PortInfo info;

        public List<Packet> QueuedPackets;
        public List<Link> ActiveLinks;
        public PacketSourceIndicator Indicator;
        public int Capacity = 5;
        public PacketSourceInfo Info;

        public bool HasUnfinishedLink()
        {
            return ActiveLinks.Exists(link => !link.Finished);
        }

        public void Initialize()
        {
            ActiveLinks = new List<Link>();

            Info.Capacity = Capacity;
            Info.NumQueuedPackets = 0;

            this.info = new PortInfo(
                this.transform.position,
                this.transform.rotation
            );

            if (Indicator == null)
            {
                var prefab = Resources.Load<GameObject>("PacketSourceIndicator");
                var indicator = Instantiate(prefab, this.transform, false);

                Indicator = indicator.GetComponent<PacketSourceIndicator>();
            }

            Indicator.UpdatePacketSourceInfo(Info);
        }

        public void EnqueuePacket(Packet p)
        {
            if (QueuedPackets.Count < Capacity)
            {
                QueuedPackets.Add(p);

                OnNewPacketEnqued(p);
            }
            else
            {
                // Drop packet.
                GameManager.ReportPacketDropped(p);

                OnPacketDropped(p);
            }
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
            if (p != null && p.Destination == t.Address)
            {
                var packet = DequeuePacket();
                l.EnqueuePacket(packet);
                packet.OnDequeuedFromPort(this, l);
                OnTransmissionStarted(l);
            }   
        }

        public virtual void OnLinkStarted(Link l)
        {
            ActiveLinks.Add(l);

            // Listen for sever events.
            l.OnSever += (SeverCause cause, float totalLength) =>
            {
                OnTransmissionSevered(cause, l);
            };
        }

        public virtual void OnLinkEstablished(Link l, PacketSink t)
        {
            FindAndSendPacketTo(l, t);
        }

        protected virtual void OnEmptied()
        {
        }

        protected virtual void OnTransmissionStarted(Link l)
        {
            Info.NumQueuedPackets--;

            Indicator.UpdatePacketSourceInfo(Info);
        }

        protected virtual void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            ActiveLinks.RemoveAt(ActiveLinks.FindIndex(
                link => link.GetInstanceID() == severedLink.GetInstanceID()));
        }

        protected virtual void OnNewPacketEnqued(Packet p)
        {
            Info.NumQueuedPackets++;

            Indicator.UpdatePacketSourceInfo(Info);
        }

        protected virtual void OnPacketDropped(Packet p)
        {

        }
    }
}
