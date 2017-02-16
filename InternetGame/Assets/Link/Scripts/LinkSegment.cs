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
        public float Thickness;
        public bool Saturated;
        public bool IsUnseverableSegment;
        public bool IsNumb = false;
        public float GraduallyMoveRate = 1.0f;

        public float SeverGracePeriod = 1.0f; // In seconds.

        public GameObject CutBoxContainer;
        public Renderer[] ModelRenderers;

        public Vector3 From;
        public Vector3 To;

        public virtual void Initialize()
        {
            var cutBoxTransform = this.transform.FindChild("CutBox");
            if (cutBoxTransform != null)
            {
                CutBoxContainer = cutBoxTransform.gameObject;
                var cutBox = CutBoxContainer.AddComponent<LinkSegmentCutBox>();
                cutBox.Initialize(this);
            }

            var modelsTransform = this.transform.FindChild("Models");
            if (modelsTransform != null)
            {
                ModelRenderers = modelsTransform.GetComponentsInChildren<Renderer>();
            }
            else
            {
                ModelRenderers = new Renderer[] { GetComponent<Renderer>() };
            }
        }

        public virtual void SetBetween(Vector3 from, Vector3 to, float segmentThickness, float segmentLength = -1.0f)
        {
            if (segmentLength < 0)
            {
                segmentLength = Vector3.Distance(from, to);
            }

            // Make as long as the pointer has traveled.
            transform.localScale = new Vector3(segmentThickness, segmentThickness, segmentLength);
            // Rotate the link to align with the gap between the two points.
            transform.rotation = Quaternion.LookRotation(to - from);
            // Position in between the two points.
            transform.position = (from + to) / 2;

            Thickness = segmentThickness;
            Length = segmentLength;
            From = from;
            To = to;
        }

        private IEnumerator AnimateMoveToBetween(Vector3 start, Vector3 end)
        {
            float startTime = Time.fixedTime;
            float t = 0.0f;
            Vector3 originalStart = From;
            Vector3 originalEnd = To;
            float segmentThickness = transform.localScale.x;

            while (t < 1.0f)
            {
                // Move the segment along from its original position to the intended destination.
                t = (Time.fixedTime - startTime) * GraduallyMoveRate;
                Vector3 intermediateStart = Vector3.Lerp(originalStart, start, t);
                Vector3 intermediateEnd = Vector3.Lerp(originalEnd, end, t);

                SetBetween(intermediateStart, intermediateEnd, segmentThickness);

                yield return null;
            }

            // Finally set at the intended destination.
            SetBetween(start, end, segmentThickness);
        }

        public virtual void GraduallyMoveToBetween(Vector3 from, Vector3 to)
        {
            StartCoroutine(AnimateMoveToBetween(from, to));
        }

        public virtual void SetModelMaterials(Material m)
        {
            foreach (Renderer r in ModelRenderers)
            {
                r.material = m;
            }
        }

        public virtual Color GetColor()
        {
            return ModelRenderers[0].material.color;
        }

        public virtual void Saturate(Material m)
        {
            SetModelMaterials(m);

            Saturated = true;
        }

        public virtual void Desaturate(Material m)
        {
            SetModelMaterials(m);

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
                && !(l.State == LinkState.EarlyTerminated)
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
            var cursor = cursorParent.GetComponentInParent<Cursor>();
            if (cursor != null)
            {
                cursor.OnEnterCut(new CursorEventArgs
                {
                    senderId = this.ParentLink.GetInstanceID()
                });
            }
        }

        private void MakeCursorInactive(Collider cursorParent)
        {
            var cursor = cursorParent.GetComponentInParent<Cursor>();
            if (cursor != null)
            {
                cursor.OnExitCut(new CursorEventArgs
                {
                    senderId = this.ParentLink.GetInstanceID()
                });
            }
        }
    }

}