using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkSegment : MonoBehaviour
    {
        public Link ParentLink;
        public Material DefaultMaterial;
        public float Length;
        public bool Saturated;
        public bool IsUnseverableSegment;

        public float SeverGracePeriod = 1.0f; // In seconds.

        public void Saturate(Material m)
        {
            GetComponent<Renderer>().material = m;

            Saturated = true;
        }

        public void Desaturate(Material m)
        {
            GetComponent<Renderer>().material = m;

            Saturated = false;
        }

        public bool LinkIsAllowedToBeSevered(Link l)
        {
            return (!(l.State == LinkState.UnderConstruction)
                && Time.fixedTime > ParentLink.FinishedTime + SeverGracePeriod) ;
        }

        public void OnTriggerEnter(Collider col)
        {
            if (!IsUnseverableSegment &&
                col.gameObject.CompareTag("Player") && 
                LinkIsAllowedToBeSevered(ParentLink))
            {
                ParentLink.Sever(SeverCause.Player, this);
            }
        }
    }

}