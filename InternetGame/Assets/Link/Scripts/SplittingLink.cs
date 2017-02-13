using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class SplittingLink : Link
    {
        public GameObject BurnTrail;
        public float LinkBurnDuration = 3.2f;
        public float SegmentBurnOverlap = 0.8f;

        private Color burnColor = Color.black;
        private Color virusBurnColor = Color.red;

        private bool isAnimatingDestruction;
        private float segmentBurnDuration;

        private bool leftSideFinishedBurning = false;
        private bool rightSideFinishedBurning = false;

        private System.Random randomSource;

        private void Start()
        {
            BurnTrail = Resources.Load<GameObject>("LinkBurnTrail");
            randomSource = new System.Random();
        }

        public override void AnimateAndDestroy(SeverCause cause, LinkSegment linkSegment)
        {
            base.AnimateAndDestroy(cause, linkSegment);

            isAnimatingDestruction = true;

            switch (cause)
            {
                case SeverCause.TransmissionFinished:
                case SeverCause.VirusTransmitted:
                case SeverCause.PlayerPreventedVirus:
                    segmentBurnDuration = 1.0f;
                    SegmentBurnOverlap = 1.0f;

                    if (cause == SeverCause.VirusTransmitted)
                    {
                        burnColor = virusBurnColor;
                    }
                    else
                    {
                        burnColor = Packet.Color;
                    }

                    // Since we are burning all at once, we need to set this
                    // variable so that the link gets cleaned up even though the burning
                    // is only going in one direction.
                    leftSideFinishedBurning = true;

                    var burnAllAtOnce = BurnTag(0, true);
                    StartCoroutine(burnAllAtOnce);

                    break;
                default:
                    segmentBurnDuration = (LinkBurnDuration / Segments.Count) / (1.0f - SegmentBurnOverlap);

                    // Start burning at the point we severed the link.
                    int leftIndex = Segments.FindIndex(candidate => candidate.GetInstanceID() == linkSegment.GetInstanceID());
                    int rightIndex = leftIndex + 1;

                    var burnLeft = BurnTag(leftIndex, false);
                    var burnRight = BurnTag(rightIndex, true);

                    if (rightIndex < Segments.Count)
                    {
                        // Only start the increasing burn animation if the severed segment isn't the end.
                        StartCoroutine(burnRight);
                    }
                    else
                    {
                        // Make sure that if we didn't start the increasing burn animation that we set
                        // the variables we otherwise would have.
                        rightSideFinishedBurning = true;
                        Connector.Fade();
                    }
                    StartCoroutine(burnLeft);
                    break;
            }
            
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

        private bool isEndSegment(int i, bool increasing)
        {
            return (increasing && i >= Segments.Count - 1) || (!increasing && i == 0);
        }

        private IEnumerator BurnTag(int i, bool increasing)
        {
            var segment = Segments[i];
            segment.Numb();

            bool isEnd = isEndSegment(i, increasing);

            var dupMaterial = new Material(segment.Model.GetComponent<Renderer>().material);
            segment.Model.GetComponent<Renderer>().material = dupMaterial;

            var startColor = dupMaterial.color;
            var endColor = Color.black;
            endColor.a = 0.0f;

            var startTime = Time.fixedTime;

            var burnTimeScalar = 1.0f / segmentBurnDuration;
            var tagThreshold = 1.0f - SegmentBurnOverlap;
            // Whether this coroutine has tagged the next segment's.
            bool tagged = false;

            var burnTrail = Instantiate(BurnTrail, Segments[i].transform, false);
            var burnTrailSystem = burnTrail.GetComponent<ParticleSystem>();
            burnTrailSystem.Pause();

            burnTrail.transform.localPosition = new Vector3(0, 0, segment.Length);
            burnTrailSystem.randomSeed = (uint) randomSource.Next(10000);

            // Make the particle effect fill the segment.
            var shape = burnTrailSystem.shape;
            shape.length = segment.Length;

            // Set burn color of particles.
            var mainSettings = burnTrailSystem.main;
            mainSettings.startColor = burnColor;

            burnTrailSystem.Play();

            // Burn segment.
            float t = 0;
            while (t <= 1.0f)
            {
                t = (Time.fixedTime - startTime) * burnTimeScalar;

                if (!isEnd && !tagged && t > tagThreshold)
                {
                    // Tag neighboring segment.
                    if (increasing)
                    {
                        StartCoroutine(BurnTag(i + 1, true));
                    }
                    else if (!increasing)
                    {
                        StartCoroutine(BurnTag(i - 1, false));
                    }

                    tagged = true;
                }
                else if (isEnd && increasing && !tagged)
                {
                    // The last segment should burn the connector too.
                    Connector.Fade();

                    tagged = true;
                }

                Color currentColor = Color.Lerp(startColor, endColor, t);
                dupMaterial.color = currentColor;

                yield return null;
            }

            // Stop the particle system, wait a second, then disable the link.
            burnTrail.GetComponent<ParticleSystem>().Stop();
            yield return new WaitUntil(() => burnTrailSystem.particleCount == 0);
            segment.gameObject.SetActive(false);

            if (isEnd)
            {
                OnSegmentFinishedBuring(increasing);
            }
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
