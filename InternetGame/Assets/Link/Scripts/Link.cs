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
        PlayerPreventedVirus,
        TransmissionFinished,
        UnfinishedLink,
        VirusTransmitted
    }

    public class Link : MonoBehaviour
    {
        // Public state.
        public float SegmentAddInterval;
        public float SegmentMinLength = 0.01f; // Meters
        public GameObject LinkSegmentPrefab;
        public float SeedTransmissionProgress = 5.0f;
        // Ends of link are unseverable because they are in close proximity to other links.
        public float UnseverableSegmentThreshold = 0.4f; // Meters
        public float InitialLinkLength = 0.3f; // Meters
        public float TaperDelay = 0.2f;
        public float TaperLength = .75f; // Meters
        public float TaperedDiameter = 0.01f; // Meters
		public float UntaperedDiameter = 0.03f;
        public Connector Connector;
        public int SlopeWindow = 3;

        public delegate void SeverHandler(Link severed, SeverCause cause, float totalLength);
        public event SeverHandler OnSever;

        public delegate void OnConstructionProgressHandler(float deltaLength, float totalLengthSoFar);
        public event OnConstructionProgressHandler OnConstructionProgress;

        public delegate void OnTransmissionStartedHandler(Link l, Packet p);
        public event OnTransmissionStartedHandler OnTransmissionStarted;

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
        public Vector3 RecentSlope;

        public bool AdjustedInitialSegments = false;
        public bool IsTransmittingPacket;
        public Packet Packet;
        public float TransmissionProgress;
        public LinkState State;

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
        public virtual void Initialize(PacketSource source, Connector connector)
        {
            IsTransmittingPacket = false;

            // Make container for link segments.
            linkSegmentContainer = new GameObject("Segments");
            linkSegmentContainer.transform.parent = this.transform;

            Connector = connector;

            float currentTime = GameManager.GetInstance().GameTime();

            StartTime = currentTime;
            Finished = false;
            FinishedTime = null;
            TotalLength = 0.0f;
            Severed = false;
            Segments = new List<LinkSegment>();

            possibleDestinations = new List<PacketSink>();

            lastSegmentAddTime = currentTime;
            lastSegmentEnd = source.LinkConnectionPoint.position;

            RecentSlope = Vector3.zero;

            source.OnLinkStarted(this);
            
            // Finds and alerts all ports that might be destination for
            // the current link.
            if (Packet != null)
            {
                AlertPacketSinksOfPacket(Packet);
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

        public virtual void AnimateAndDestroy(SeverCause cause, LinkSegment severedSegment)
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

        public static Vector3 CubicLerp(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector3 QuadraticLerp(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
        }

        private void AdjustInitialLinkSegments(LinkSegment lastAdjustedSegment, int numSegmentsToAdjust)
        {
            AdjustedInitialSegments = true;

            Vector3 start = Source.LinkConnectionPoint.position;
            Vector3 center = Segments[Segments.Count / 2].transform.position;
            Vector3 end = lastAdjustedSegment.To;

            int adjustedSegments = 0;
            foreach (LinkSegment segment in Segments)
            {
                Vector3 from = QuadraticLerp(start, center, end, (float) (adjustedSegments) / numSegmentsToAdjust);
                Vector3 to = QuadraticLerp(start, center, end, (float) (adjustedSegments + 1.0f) / numSegmentsToAdjust);

                segment.GraduallyMoveToBetween(from, to);

                adjustedSegments++;

                if (segment == lastAdjustedSegment)
                {
                    return;
                }
            }
        }

        private LinkSegment MakeSegmentBetween(Vector3 from, Vector3 to)
        {
            var segment = Instantiate(LinkSegmentPrefab, linkSegmentContainer.transform);
            float segmentLength = Vector3.Distance(from, to);

            LinkSegment linkSegment = segment.GetComponent<LinkSegment>();
            if (linkSegment == null)
            {
                Debug.LogError("No link segment script found on link segment prefab");
            }

			float segmentThickness = UntaperedDiameter;
			float segmentStartThickness = -1.0f;
            if (TotalLength - segmentLength < TaperLength + TaperDelay)
            {
                // Make segment progressively thicker.
				segmentThickness = Mathf.Lerp(TaperedDiameter, UntaperedDiameter,
                    (TotalLength - TaperDelay) / TaperLength);
				segmentStartThickness = Mathf.Lerp (TaperedDiameter, UntaperedDiameter,
					(TotalLength - segmentLength - TaperDelay) / TaperLength);
            }

            linkSegment.ParentLink = this;
            linkSegment.Initialize();

            linkSegment.SetBetween(from, to, segmentThickness, UntaperedDiameter,
								   segmentLength, segmentStartThickness);

            return linkSegment;
        }

        /// <summary>
        /// Adds a new segment to the current Link.
        /// </summary>
        private void AddNewSegment(Vector3 nextPosition)
        {
            // Time to add a new interval.
            var segmentLength = Vector3.Distance(lastSegmentEnd, nextPosition);

            if (segmentLength >= SegmentMinLength)
            {
                TotalLength += segmentLength;

                LinkSegment linkSegment = MakeSegmentBetween(lastSegmentEnd, nextPosition);

                // Set initial color to be desaturated.
                linkSegment.Desaturate(Packet.Payload.Destaturated);

                Segments.Add(linkSegment);

                lastSegmentAddTime = GameManager.GetInstance().GameTime();
                lastSegmentEnd = nextPosition;

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
            if (!Severed)
            {
                Finished = true;
                Severed = true;
                FinishedTime = GameManager.GetInstance().GameTime();
                State = LinkState.Severed;

                UndoAlertPacketSinksOfPacket();

                if (cause == SeverCause.Player)
                {
                    if (Packet.Payload is Virus)
                    {
                        // Add some additional information if the player prevented a virus, specifically.
                        cause = SeverCause.PlayerPreventedVirus;
                    }

                    Packet.OnDropped(PacketDroppedCause.Severed);
                }

                if (OnSever != null)
                {
                    OnSever.Invoke(this, cause, TotalLength);
                }
            }

            AnimateAndDestroy(cause, severedSegment);

            Packet = null;
            Source = null;
            Sink = null;
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
                    State = LinkState.AwaitingPacket;

                    Sink = t;

                    MakeEndsUnseverable(Segments, UnseverableSegmentThreshold);

                    Source.OnLinkEstablished(this, Sink);
                    Sink.OnLinkEstablished(this, Source);

                    StartTransmission();
                }
                else
                {
                    // End somewhere other than a sink.
                    State = LinkState.EarlyTerminated;
                    
                    GameManager.GetInstance().ReportPacketDropped(Packet);
                    Packet.OnDropped(PacketDroppedCause.EarlyTermination);
                    Packet = null;
                }

                UndoAlertPacketSinksOfPacket();

                Finished = true;
                FinishedTime = GameManager.GetInstance().GameTime();
            }

            return TotalLength;
        }

        /// <summary>
        /// Adds a packet to the link, but does not start transmission.
        /// </summary>
        /// <param name="p">The Packet to add.</param>
        public void EnqueuePacket(Packet p)
        {
            Packet = p;
        }

        /// <summary>
        /// Begins the transmission of the packet that was enqueued.
        /// </summary>
        public void StartTransmission()
        {
            if (Packet != null)
            {
                // First desaturate the link.
                DesaturateSegments(0, Segments.Count, Packet);

                State = LinkState.TransmittingPacket;
                TransmissionProgress = SeedTransmissionProgress;
                NeededProgress = (Packet.Payload.Size * TotalLength);
                IsTransmittingPacket = true;

                if (OnTransmissionStarted != null)
                {
                    OnTransmissionStarted.Invoke(this, Packet);
                }
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
                DesaturateSegments(0, Segments.Count, Packet);

                // Clean up packet.
                Destroy(Packet.gameObject);

                var cause = Packet.Payload is Virus ?
                    SeverCause.VirusTransmitted : 
                    SeverCause.TransmissionFinished;

                // Sever at first link segment.
                this.Sever(cause, Segments[0]);

                PacketStart = 0;
                PacketEnd = 0;
                TransmissionProgress = 0.0f;
                NeededProgress = 0.0f;

                IsTransmittingPacket = false;
                Packet = null;
            }
        }

        private void SaturateSegments(int start, int end, Packet p)
        {
            for (int i = start; i < end; i++)
            {
                Segments[i].Saturate(p.Payload.Saturated);
            }
        }

        private void DesaturateSegments(int start, int end, Packet p)
        {
            for (int i = start; i < end; i++)
            {
                Segments[i].Desaturate(p.Payload.Destaturated);
            }
        }

        private void UpdateSlope()
        {
            // Update slope.
            if (Segments.Count > SlopeWindow)
            {
                LinkSegment phasedOutSegment = Segments[Segments.Count - SlopeWindow - 1];
                Vector3 phasedOutVector = phasedOutSegment.To - phasedOutSegment.From;

                // Reuses computation from previous update.
                Vector3 reusableComputation = (RecentSlope - (phasedOutVector / SlopeWindow));

                LinkSegment newSegment = Segments[Segments.Count - 1];
                Vector3 newVector = newSegment.To - newSegment.From;

                RecentSlope = reusableComputation + (newVector / SlopeWindow);
            }
            else
            {
                Vector3 accumulator = Vector3.zero;
                foreach (LinkSegment segment in Segments)
                {
                    accumulator += (segment.To - segment.From);
                }
                RecentSlope = accumulator / Segments.Count;
            }
        }

        public void Update()
        {
            if (!GameManager.GetInstance().IsPaused)
            {
                switch (State)
                {
                    case LinkState.UnderConstruction:
                        if (GameManager.GetInstance().GameTime() - lastSegmentAddTime >= SegmentAddInterval)
                        {
                            AddNewSegment(Connector.LinkPointer.position);

                            UpdateSlope();
                        }

                        if (!AdjustedInitialSegments && TotalLength > InitialLinkLength)
                        {
                            AdjustInitialLinkSegments(Segments[Segments.Count - 1], Segments.Count);
                        }
                        break;
                    case LinkState.TransmittingPacket:
                        int oldStart = PacketStart;
                        int oldEnd = PacketEnd;

                        // Incrememnt progress
                        TransmissionProgress += Bandwidth * Time.deltaTime;

                        float percentageProgress = TransmissionProgress / NeededProgress;
                        percentageProgress = percentageProgress > 1.0f ? 1.0f : percentageProgress;

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
                            DesaturateSegments(oldStart, Math.Min(PacketStart, oldEnd), Packet);

                            // Activate these segments.
                            SaturateSegments(Math.Max(oldEnd, PacketStart), PacketEnd, Packet);
                        }

                        break;
                }
            }
        }
    }
}
