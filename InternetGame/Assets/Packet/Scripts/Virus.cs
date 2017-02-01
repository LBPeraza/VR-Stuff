using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public abstract class Virus : Packet
    {
        public float Damage;

        public override void OnDequeuedFromPort(PacketSource p, Link l)
        {
            base.OnDequeuedFromPort(p, l);
            l.OnSever += OnSever;
        }

        public virtual void OnSever(float totalLength)
        {
            GameManager.ReportStoppedVirus(this);
        }
        
        public override void OnDequeuedFromLink(Link l, PacketSink p)
        {
            GameManager.AddVirus(this);
        }
    }
}
