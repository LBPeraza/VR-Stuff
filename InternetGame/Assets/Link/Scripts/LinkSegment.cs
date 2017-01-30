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
            if (DefaultMaterial == null)
            {
                DefaultMaterial = GetComponent<Renderer>().material;
            }

            var materials = GetComponent<Renderer>().materials;
            materials[0] = m;
            GetComponent<Renderer>().materials = materials;

            Saturated = true;
        }

        public void Desaturate()
        {
            if (Saturated && DefaultMaterial != null)
            {
                var materials = GetComponent<Renderer>().materials;
                materials[0] = DefaultMaterial;

                GetComponent<Renderer>().materials = materials;

                Saturated = false;
            }
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
                ParentLink.Sever();
            }
        }
    }

}