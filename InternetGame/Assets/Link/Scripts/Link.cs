using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace InternetGame
{
    public enum LinkState
    {
        Default,
        UnderConstruction,
        Severed,
        Completed,
        AwaitingPacket,
        TransmittingPacket,
        EarlyTerminated
    }

    public class Link : MonoBehaviour
    {
        // Public state.
        public float SegmentAddInterval;
        public float SegmentMinLength = 0.005f;
        public GameObject LinkSegmentPrefab;
        public Transform Pointer;

        public delegate void SeverHandler(float totalLength);
        public event SeverHandler OnSever;

        public delegate void OnConstructionProgressHandler(float deltaLength, float totalLengthSoFar);
        public event OnConstructionProgressHandler OnConstructionProgress;

        public float Bandwidth; // Meters/second.
        public PacketSource Source;
        public PacketSink Sink;

        // Read-only properties.
        public List<LinkSegment> Segments;
        public float StartTime;
        public bool Finished;
        public float? FinishedTime;
        public bool Severed;
        public float TotalLength;
        public bool IsTransmittingPacket;
        public Packet Packet;
        public float TransmissionProgress;
        public LinkState State;
        public float SeedTransmissionProgress = 5.0f;
        // Ends of link are unseverable because they are in close proximity to other links.
        public float UnseverableSegmentThreshold = 0.4f; // Meters

        // Private state.
        private GameObject linkSegmentContainer;
        private float lastSegmentAddTime;
        private Vector3 lastSegmentEnd;
        private bool isAnimatingDestruction;
        private int PacketStart, PacketEnd;
        public float NeededProgress;

        /// <summary>
        /// Initializes and starts the link, using the given Transform reference as the "cursor" 
        /// that the link will follow.
        /// </summary>
        /// <param name="pointer">The pointer that the link will follow.</param>
        public void Initialize(Transform pointer)
        {
            IsTransmittingPacket = false;
            isAnimatingDestruction = false;

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

            State = LinkState.UnderConstruction;
        }

        /// <summary>
        /// Early-terminates the link, alerting any subscribed delegates to the severing.
        /// </summary>
        public void Sever()
        {
            // Put packet back into source if early terminates.
            if (State == LinkState.TransmittingPacket || State == LinkState.EarlyTerminated)
            {
                Source.EnqueuePacket(Packet);
            }

            Finished = true;
            Severed = true;
            FinishedTime = Time.fixedTime;
            State = LinkState.Severed;

            if (OnSever != null)
            {
                OnSever.Invoke(TotalLength);
            }

            AnimateDestroy();
        }

        public void AnimateDestroy()
        {
            isAnimatingDestruction = true;
            var fadeCoroutine  = Fade();
            StartCoroutine(fadeCoroutine);
        }

        IEnumerator Fade()
        {
            float originalThickness = Segments.Count > 0 ? Segments[0].transform.localScale.x : .03f;
            for (float f = originalThickness; f >= 0; f -= 0.001f)
            {
                foreach (LinkSegment segment in Segments)
                {
                    segment.transform.localScale = new Vector3(f, f, segment.transform.localScale.z);
                }
                yield return null;
            }

            Destroy(this.gameObject);
        }

        private void MakeEndsUnseverable(List<LinkSegment> segments, 
            float lengthToMakeUnseverable)
        {
            float distanceTraversed = 0.0f;

            foreach (LinkSegment segment in segments)
            {
                distanceTraversed += segment.Length;
                if (distanceTraversed <= lengthToMakeUnseverable)
                {
                    segment.IsUnseverableSegment = true;
                } else
                {
                    break;
                }
            }

            distanceTraversed = 0.0f;
            for (int i = segments.Count - 1; i >= 0; i --)
            {
                var segment = segments[i];
                distanceTraversed += segment.Length;
                if (distanceTraversed <= lengthToMakeUnseverable)
                {
                    segment.IsUnseverableSegment = true;
                }
                else
                {
                    break;
                }
            }
            
        }
             
        /// <summary>
        /// Ends the link, returning the total length of the link.
        /// </summary>
        /// <param name="t">The sink of the link.</param>
        /// <returns>Total length of link used.</returns>
        public float End(PacketSink t = null)
        {
            if (State == LinkState.UnderConstruction)
            {
                if (t != null)
                {
                    // End at the sink.
                    Pointer = t.transform;
                    AddNewSegment();

                    State = LinkState.AwaitingPacket;

                    Sink = t;

                    Source.OnLinkEstablished(this, Sink);
                    Sink.OnLinkEstablished(this, Source);

                    MakeEndsUnseverable(Segments, UnseverableSegmentThreshold);
                }
                else
                {
                    // Ended somewhere other than a sink.
                    State = LinkState.EarlyTerminated;
                }

                Finished = true;
                FinishedTime = Time.fixedTime;
            }

            return TotalLength;
        }

        /// <summary>
        /// Adds a packet to the link.
        /// </summary>
        /// <param name="p">The Packet to add.</param>
        public void EnqueuePacket(Packet p)
        {
            if (State == LinkState.AwaitingPacket && !IsTransmittingPacket) {
                Packet = p;
                IsTransmittingPacket = true;
                TransmissionProgress = SeedTransmissionProgress;
                NeededProgress = (Packet.Size * TotalLength);
                State = LinkState.TransmittingPacket;
            }
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

                Segments.Add(linkSegment);

                lastSegmentAddTime = Time.fixedTime;
                lastSegmentEnd = currentPointerPos;

                // Notify handlers of progress.
                if (OnConstructionProgress != null)
                {
                    OnConstructionProgress.Invoke(segmentLength, TotalLength);
                }
            }
        }

        public void EndTransmission()
        {
            if (State == LinkState.TransmittingPacket)
            {
                Packet.OnDequeuedFromLink(this, Sink);

                // Desaturate all segments.
                DesaturateSegments(0, Segments.Count);

                PacketStart = 0;
                PacketEnd = 0;
                TransmissionProgress = 0.0f;
                NeededProgress = 0.0f;

                IsTransmittingPacket = false;
                Packet = null;

                State = LinkState.AwaitingPacket;
            }
        }

        private void SaturateSegments(int start, int end, Packet p)
        {
            for (int i = start; i < end; i++)
            {
                Segments[i].Saturate(p.Indicator);
            }
        }

        private void DesaturateSegments(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                Segments[i].Desaturate();
            }
        }

        public void Update()
        {
            switch (State)
            {
                case LinkState.UnderConstruction:
                    if (Time.fixedTime - lastSegmentAddTime >= SegmentAddInterval)
                    {
                        AddNewSegment();
                    }
                    break;
                case LinkState.TransmittingPacket:
                    int oldStart = PacketStart;
                    int oldEnd = PacketEnd;

                    // Incrememnt progress
                    TransmissionProgress += Bandwidth * Time.deltaTime;

                    if (TransmissionProgress >= NeededProgress)
                    {
                        // Transmission completed.
                        EndTransmission();
                    }
                    else
                    {
                        float percentageProgress = TransmissionProgress / NeededProgress;
                        PacketEnd = (int)(Segments.Count * percentageProgress);
                        PacketStart = 0;

                        // Deactivate these segments.
                        DesaturateSegments(oldStart, Math.Min(PacketStart, oldEnd));

                        // Activate these segments.
                        SaturateSegments(Math.Max(oldEnd, PacketStart), PacketEnd, Packet);
                    }
                    
                    break;
            }
            
        }
    }
}
