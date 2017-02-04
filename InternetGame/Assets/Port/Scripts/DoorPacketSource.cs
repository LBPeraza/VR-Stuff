using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class DoorPacketSource : PacketSource
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void OnNewPacketOnDeck(Packet p)
        {
            base.OnNewPacketOnDeck(p);

            if (!HasUnfinishedLink())
            {
                //StartFlashing((Color)PacketSpawner.AddressToColor[p.Destination]);
            }
        }

        protected override void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            base.OnTransmissionSevered(cause, severedLink);

            if (cause == SeverCause.UnfinishedLink)
            {
                // When the player doesnt finish a link, we need to stop flashing.
                //EndFlashing();
            }
        }

        protected override void OnTransmissionStarted(Link l, Packet p)
        {
            base.OnTransmissionStarted(l, p);

            if (IsEmpty())
            {
                //EndFlashing();
            }
            else
            {
               // StartFlashing((Color)PacketSpawner.AddressToColor[Peek().Destination]);
            }
        }
    }
}

