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
        public bool IsNumb = false;

        public float SeverGracePeriod = 1.0f; // In seconds.

        public GameObject CutBoxContainer;
        public GameObject Model;

        public void Initialize()
        {
            var cutBoxTransform = this.transform.FindChild("CutBox");
            if (cutBoxTransform != null)
            {
                CutBoxContainer = cutBoxTransform.gameObject;
                var cutBox = CutBoxContainer.AddComponent<LinkSegmentCutBox>();
                cutBox.Initialize(this);
            }

            var modelTransform = this.transform.FindChild("Model");
            if (modelTransform != null)
            {
                Model = modelTransform.gameObject;
            }
            else
            {
                Model = this.gameObject;
            }
        }

        public virtual void Saturate(Material m)
        {
            Model.GetComponent<Renderer>().material = m;

            Saturated = true;
        }

        public virtual void Desaturate(Material m)
        {
            Model.GetComponent<Renderer>().material = m;

            Saturated = false;
        }

        public void Numb()
        {
            // Make unresponsive to sever events.
            IsNumb = true;
        }

        public bool LinkIsAllowedToBeSevered(Link l)
        {
            return (
                !IsNumb 
                && !(l.State == LinkState.UnderConstruction)
                && Time.fixedTime > ParentLink.FinishedTime + SeverGracePeriod) ;
        }
        
        public void OnCutBoxEnter(Collider col)
        {
            if (!IsUnseverableSegment &&
                col.gameObject.CompareTag("Player") &&
                LinkIsAllowedToBeSevered(ParentLink))
            {
                MakeCursorScissors(col);
            }
        }

        public void OnCutBoxExit(Collider col)
        {
            MakeCursorInactive(col);
        }

        public void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("LinkCutBox"))
            {
                // Ignore the cut box.
                return;
            }

            if (!IsUnseverableSegment &&
                col.gameObject.CompareTag("Player") && 
                LinkIsAllowedToBeSevered(ParentLink))
            {
                MakeCursorScissors(col);
                ParentLink.Sever(SeverCause.Player, this);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            MakeCursorInactive(other);
        }

        private void MakeCursorScissors(Collider cursorParent)
        {
            var cursor = cursorParent.transform.parent.GetComponent<Cursor>();
            if (cursor != null)
            {
                cursor.OnEnterCut(new CursorEventArgs
                {
                    senderId = this.GetInstanceID()
                });
            }
        }

        private void MakeCursorInactive(Collider cursorParent)
        {
            var cursor = cursorParent.transform.parent.GetComponent<Cursor>();
            if (cursor != null)
            {
                cursor.OnExitCut(new CursorEventArgs
                {
                    senderId = this.GetInstanceID()
                });
            }
        }
    }

}