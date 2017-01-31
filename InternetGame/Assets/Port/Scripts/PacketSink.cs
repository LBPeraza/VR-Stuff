using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSink : MonoBehaviour
    {
        public string Address;
        public Link ActiveLink;

        public virtual void OnBecameOptionForLink(Link l)
        {

        }

        public virtual void OnNoLongerOptionForLink(Link l)
        {

        }

        public virtual void OnLinkEstablished(Link l, PacketSource s)
        {
        }

        public virtual void OnTransmissionStarted(Link l)
        {
            // Listen for sever events.
            l.OnSever += (float totalLength) =>
            {
                OnTransmissionSevered(l);
            };
        }

        protected virtual void OnTransmissionSevered(Link severedLink)
        {
            ActiveLink = null;
        }
    }
}

