using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkSegment : MonoBehaviour
    {
        public Link ParentLink;
        public float Length;

        private bool isActive {
            get
            {
                return ParentLink.Finished && !ParentLink.Severed;
            }
        }

        public void OnTriggerEnter(Collider col)
        {
            if (this.isActive && col.gameObject.CompareTag("Player"))
            {
                ParentLink.Sever();
            }
        }
    }

}