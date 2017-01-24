using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Link : MonoBehaviour
    {
        // Public state.
        public float SegmentAddInterval;
        public float SegmentMinLength = 0.1f;
        public GameObject LinkSegmentPrefab;
        public Transform Pointer;

        // Read-only properties.
        public List<LinkSegment> Segments;
        public float StartTime;
        public bool Finished;
        public float? FinishedTime;
        public bool Severed;
        public float TotalLength;

        // Private state.
        private GameObject linkSegmentContainer;
        private float lastSegmentAddTime;
        private Vector3 lastSegmentEnd;

        /// <summary>
        /// Initializes and starts the link, using the given Transform reference as the "cursor" 
        /// that the link will follow.
        /// </summary>
        /// <param name="pointer">The pointer that the link will follow.</param>
        public void Initialize(Transform pointer)
        {
            // Make container for link segments.
            linkSegmentContainer = new GameObject("Segments");
            linkSegmentContainer.transform.parent = this.transform;

            Pointer = pointer;

            StartTime = Time.fixedTime;
            Finished = false;
            FinishedTime = null;
            TotalLength = 0.0f;
            Severed = false;
            Segments = new List<LinkSegment>();

            lastSegmentAddTime = Time.fixedTime;
            lastSegmentEnd = pointer.position;
        }

        /// <summary>
        /// Ends the link, returning the total length of the link.
        /// </summary>
        /// <param name="endPoint">Optional transform to override the endpoint of the link.</param>
        /// <returns>Total length of link used.</returns>
        public float End(Transform endPoint = null)
        {
            if (endPoint)
            {
                Pointer = endPoint;
            }
            AddNewSegment();

            Finished = true;
            FinishedTime = Time.fixedTime;

            return TotalLength;
        }

        private void AddNewSegment()
        {
            // Time to add a new interval.
            var currentPointerPos = Pointer.position;
            var segmentLength = Vector3.Distance(lastSegmentEnd, currentPointerPos);

            if (segmentLength >= SegmentMinLength)
            {
                TotalLength += segmentLength;

                var segment = Instantiate(LinkSegmentPrefab);

                var linkSegment = segment.AddComponent<LinkSegment>();
                linkSegment.ParentLink = this;
                linkSegment.Length = segmentLength;

                segment.transform.parent = linkSegmentContainer.transform;
                segment.transform.position = (lastSegmentEnd + currentPointerPos) / 2;
                // Make as long as the pointer has traveled.
                segment.transform.localScale = new Vector3(segment.transform.localScale.x, segment.transform.localScale.y, segmentLength);
                // Rotate the link to align with the gap between the two points.
                segment.transform.rotation = Quaternion.LookRotation(currentPointerPos - lastSegmentEnd);

                lastSegmentAddTime = Time.fixedTime;
                lastSegmentEnd = currentPointerPos;
            }
        }

        public void Update()
        {
            if (!Finished && Time.fixedTime - lastSegmentAddTime >= SegmentAddInterval)
            {
                AddNewSegment();
            }
        }
    }
}
