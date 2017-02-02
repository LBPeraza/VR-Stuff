using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkSegmentCutBox : MonoBehaviour
    {
        public LinkSegment Parent;

        public void Initialize(LinkSegment parent)
        {
            Parent = parent;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Parent.OnCutBoxEnter(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Parent.OnCutBoxExit(other);
            }
        }
    }
}

