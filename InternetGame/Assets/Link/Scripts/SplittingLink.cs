using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class SplittingLink : Link
    {
        private bool isAnimatingDestruction;

        public float LinkBurnDuration = 3.2f;
        public float SegmentBurnOverlap = 0.8f;

        private float segmentBurnDuration;

        private bool leftSideFinishedBurning = false;
        private bool rightSideFinishedBurning = false;

        public override void AnimateAndDestroy(LinkSegment linkSegment)
        {
            base.AnimateAndDestroy(linkSegment);

            isAnimatingDestruction = true;
            segmentBurnDuration = (LinkBurnDuration / Segments.Count) / (1.0f - SegmentBurnOverlap);

            // Start burning at the point we severed the link.
            int leftIndex = Segments.FindIndex(candidate => candidate.GetInstanceID() == linkSegment.GetInstanceID());
            int rightIndex = leftIndex + 1;
            
            var burnLeft = BurnTag(leftIndex, false);
            var burnRight = BurnTag(rightIndex, true);
            StartCoroutine(burnLeft);
            StartCoroutine(burnRight);
        }

        private void OnSegmentFinishedBuring(bool wasRightSide)
        {
            if (wasRightSide)
            {
                rightSideFinishedBurning = true;
            }
            else
            {
                leftSideFinishedBurning = true;
            }

            if (leftSideFinishedBurning && rightSideFinishedBurning)
            {
                Destroy(this.gameObject);
            }
        }

        private IEnumerator BurnTag(int i, bool increasing)
        {
            var segment = Segments[i];
            var dupMaterial = new Material(segment.GetComponent<Renderer>().material);
            segment.GetComponent<Renderer>().material = dupMaterial;

            var startColor = dupMaterial.color;
            var endColor = Color.black;
            endColor.a = 0.0f;

            var startTime = Time.fixedTime;

            var burnTimeScalar = 1.0f / segmentBurnDuration;
            var tagThreshold = 1.0f - SegmentBurnOverlap;
            bool tagged = false;

            // Burn segment.
            float t = 0;
            while (t <= 1.0f)
            {
                t = (Time.fixedTime - startTime) * burnTimeScalar;

                if (!tagged && t > tagThreshold)
                {
                    // Tag neighboring segment.
                    if (increasing && i < Segments.Count - 1)
                    {
                        StartCoroutine(BurnTag(i + 1, true));
                    }
                    else if (!increasing && i > 0)
                    {
                        StartCoroutine(BurnTag(i - 1, false));
                    }
                    else
                    {
                        OnSegmentFinishedBuring(increasing);
                    }

                    tagged = true;
                }

                Color currentColor = Color.Lerp(startColor, endColor, t);
                dupMaterial.color = currentColor;

                yield return null;
            }

            segment.gameObject.SetActive(false);
        }

        #region Deprecated 
        private IEnumerator Burn(int i, int segmentBlockLength, bool increasing)
        {
            while (i >= 0 && i < Segments.Count)
            {
                IEnumerator burnSegmentBlock;
                if (increasing)
                {
                    burnSegmentBlock = BurnSegmentBlock(Segments, i, Mathf.Min(Segments.Count - 1, i + segmentBlockLength));
                }
                else
                {
                    burnSegmentBlock = BurnSegmentBlock(Segments, Mathf.Max(0, i - segmentBlockLength), i);
                }
                yield return StartCoroutine(burnSegmentBlock);

                if (increasing)
                {
                    i += segmentBlockLength;
                } else
                {
                    i -= segmentBlockLength;
                }
            }
        }

        private IEnumerator BurnSegmentBlock(List<LinkSegment> segments, int i, int j)
        {
            var sampleSegment = segments[i];
            var dupMaterial = new Material(sampleSegment.GetComponent<Renderer>().material);

            var segmentRange = segments.GetRange(i, j - i + 1);
            foreach (var segment in segmentRange)
            {
                segment.GetComponent<Renderer>().material = dupMaterial;
            }

            var startColor = dupMaterial.color;
            var endColor = Color.black;

            var startTime = Time.fixedTime;

            var burnTimeScalar = 1.0f / segmentBurnDuration;

            float t = 0;
            while (t <= 1.0f)
            {
                t = (Time.fixedTime - startTime) * burnTimeScalar;
                
                Color currentColor = Color.Lerp(startColor, endColor, t);
                dupMaterial.color = currentColor;

                yield return null;
            }

            foreach (var segment in segmentRange)
            {
                segment.gameObject.SetActive(false);
            }
        }
        #endregion Deprecated
    }
}
