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

    public enum SeverCause
    {
        Player,
        TransmissionFinished,
        UnfinishedLink
    }

    public class Link : MonoBehaviour
    {
        // Public state.
        public float SegmentAddInterval;
        public float SegmentMinLength = 0.005f;
        public GameObject LinkSegmentPrefab;
        public Transform Pointer;

        public delegate void SeverHandler(SeverCause cause, float totalLength);
        public event SeverHandler OnSever;

        public delegate void OnConstructionProgressHandler(float deltaLength, float totalLengthSoFar);
        public event OnConstructionProgressHandler OnConstructionProgress;

        public delegate void OnTransmissionProgressHandler(float percentage);
        public event OnTransmissionProgressHandler OnTransmissionProgress;

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
        private int PacketStart, PacketEnd;
        public float NeededProgress;
        private List<PacketSink> possibleDestinations;

        /// <summary>
        /// Initializes and starts the link, using the given Transform reference as the "cursor" 
        /// that the link will follow.
        /// </summary>
        /// <param name="pointer">The pointer that the link will follow.</param>
        public void Initialize(PacketSource source, Transform pointer)
        {
            IsTransmittingPacket = false;

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

            possibleDestinations = new List<PacketSink>();

            lastSegmentAddTime = Time.fixedTime;
            lastSegmentEnd = pointer.position;

            source.OnLinkStarted(this);
            
            // Finds and alerts all ports that might be destination for
            // the current link.
            Packet p = source.Peek();
            if (p != null)
            {
                AlertPacketSinksOfPacket(p);
            }

            State = LinkState.UnderConstruction;
        }

        private void AlertPacketSinksOfPacket(Packet p)
        {
            foreach (PacketSink sink in GameManager.AllPacketSinks)
            {
                if (sink.Address == p.Destination)
                {
                    sink.OnBecameOptionForLink(this);
                    possibleDestinations.Add(sink);
                }
            }
        }

        private void UndoAlertPacketSinksOfPacket()
        {
            foreach (PacketSink sink in possibleDestinations)
            {
                 sink.OnNoLongerOptionForLink(this);   
            }

            possibleDestinations.Clear();
        }

        public virtual void AnimateAndDestroy(LinkSegment severedSegment)
        {
            
        }

        /// <summary>
        /// Makes the end of a current link "unseverable."
        /// </summary>
        /// <param name="segments">The length of segments to consider.</param>
        /// <param name="lengthToMakeUnseverable">The distance in meters to make unseverable.</param>
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
        /// Adds a new segment to the current Link.
        /// </summary>
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

        /// <summary>
        /// Early-terminates the link, alerting any subscribed delegates to the severing.
        /// </summary>
        public void Sever(SeverCause cause, LinkSegment severedSegment)
        {
            Finished = true;
            Severed = true;
            FinishedTime = Time.fixedTime;
            State = LinkState.Severed;

            UndoAlertPacketSinksOfPacket();

            if (OnSever != null)
            {
                OnSever.Invoke(cause, TotalLength);
            }

            Packet = null;
            Source = null;
            Sink = null;

            AnimateAndDestroy(severedSegment);
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
                    // End at a sink.
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
                    // End somewhere other than a sink.
                    State = LinkState.EarlyTerminated;
                }

                UndoAlertPacketSinksOfPacket();

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
            if (State == LinkState.AwaitingPacket && !IsTransmittingPacket)
            {
                Packet = p;
                IsTransmittingPacket = true;
                TransmissionProgress = SeedTransmissionProgress;
                NeededProgress = (Packet.Size * TotalLength);
                State = LinkState.TransmittingPacket;
            }
        }

        /// <summary>
        /// Clears link of transmission.
        /// </summary>
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

                // Sever at middle element.
                this.Sever(SeverCause.TransmissionFinished, Segments[Segments.Count / 2]);
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

                    float percentageProgress = TransmissionProgress / NeededProgress;
                    percentageProgress = percentageProgress > 100.0f ? 100.0f : percentageProgress;

                    // Notify listeners of progress.
                    if (OnTransmissionProgress != null)
                    {
                        OnTransmissionProgress.Invoke(percentageProgress);
                    }

                    if (TransmissionProgress >= NeededProgress)
                    {
                        // Transmission completed.
                        EndTransmission();
                    }
                    else
                    {
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
