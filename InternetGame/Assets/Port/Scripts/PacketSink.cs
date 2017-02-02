using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSink : MonoBehaviour
    {
        public string Address;
        public Link ActiveLink;
		public PortInfo info;

        private void Start()
        {
            // TODO: call Initialize from PortSpawner or similar.
            Initialize();
        }

        public virtual void Initialize()
        {

        }

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
            l.OnSever += (SeverCause cause, float totalLength) =>
            {
                OnTransmissionSevered(cause, l);
            };
        }

        protected virtual void OnTransmissionSevered(SeverCause cause, Link severedLink)
        {
            ActiveLink = null;
        }

		void Start() {
			this.info = new PortInfo (
				this.transform.position,
				this.transform.rotation
			);
		}
    }
}

