using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Email : Packet
    {
        public void Initialize()
        {
            this.Size = 1000;
            this.Indicator = Resources.Load<Material>("EmailIndicator"); 
        }

        public override void OnDequeuedFromLink(Link l, Port p)
        {
        }

        public override void OnDequeuedFromPort(Port p, Link l)
        {
        }

        public override void OnEnqueuedToPort(Port p)
        {
        }
    }
}
