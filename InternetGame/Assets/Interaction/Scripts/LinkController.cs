using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum LinkControllerState
    {
        Inactive,
        OverSource,
        DrawingLink
    }

    public class LinkController : MonoBehaviour
    {
        public bool IsRightHand;
        public Player Player;
        public Cursor Cursor;
        public int ObjectId;

        public LinkControllerState State;

        private GameObject CurrentLink;
        private GameObject LastLink;

        private PacketSource NearSource;
        private PacketSink NearSink;

        private CursorEventArgs cursorEventArgs;

        // Use this for initialization
        public void Initialize(Player p, bool isRightHand)
        {
            Player = p;
            IsRightHand = isRightHand;

            ObjectId = GetInstanceID();
            cursorEventArgs.senderId = ObjectId;

            if (Cursor == null)
            {
                Cursor = GetComponent<Cursor>();
            }

            State = LinkControllerState.Inactive;

            if (IsRightHand)
            {
                InputManager.RightTriggerClicked += TriggerDown;
                InputManager.RightTriggerUnclicked += TriggerUp;
            }
            else
            {
                InputManager.LeftTriggerClicked += TriggerDown;
                InputManager.LeftTriggerUnclicked += TriggerUp;
            }
        }
        

        private void AddLink()
        {
            GameObject LinkContainer = LinkFactory.CreateLink(NearSource);
            var linkSegment = LinkContainer.GetComponent<Link>();
            linkSegment.Initialize(NearSource, this.transform);

            // Listen for sever events.
            linkSegment.OnSever += LinkSegment_OnSever;
            linkSegment.OnConstructionProgress += LinkSegment_OnConstructionProgress;

            CurrentLink = LinkContainer;

            State = LinkControllerState.DrawingLink;
            Cursor.OnGrab(cursorEventArgs);
        }

        public void TriggerDown(object sender, ClickedEventArgs args)
        {
            Debug.Log("Controller (right? = " + IsRightHand + ") Trigger down fired");
            if (CurrentLink == null && NearSource != null && !Player.IsOutOfBandwidth())
            {
                AddLink();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (CurrentLink == null && other.CompareTag("Source"))
            {
                // In close proximity of packet source.
                NearSource = other.GetComponent<PacketSource>();

                State = LinkControllerState.OverSource;
                Cursor.OnEnter(cursorEventArgs);
            }
            else if (CurrentLink != null && other.CompareTag("Sink"))
            {
                // In close proximity of packet sink.
                NearSink = other.GetComponent<PacketSink>();

                // End the current link at the sink.
                var currentLinkComponent = CurrentLink.GetComponent<Link>();
                currentLinkComponent.End(NearSink);

                DestroyLink();

                State = LinkControllerState.Inactive;
                Cursor.OnExit(cursorEventArgs);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Source"))
            {
                NearSource = null;

                if (State == LinkControllerState.OverSource)
                {
                    State = LinkControllerState.Inactive;
                    Cursor.OnExit(cursorEventArgs);
                }
            }
            else if (other.CompareTag("Sink"))
            {
                NearSink = null;
            }
        }

        private void LinkSegment_OnSever(float totalLength)
        {
            // Restore the bandwidth that this link was using.
            Player.TotalBandwidth += totalLength;
        }

        private void LinkSegment_OnConstructionProgress(float deltaLength, 
            float totalLengthSoFar)
        {
            if (CurrentLink != null)
            {
                Player.TotalBandwidth -= deltaLength;
                if (Player.IsOutOfBandwidth())
                {
                    CurrentLink.GetComponent<Link>().End();

                    Cursor.OnExit(cursorEventArgs);
                    State = LinkControllerState.Inactive;
                }
            }
        }

        private void DestroyLink()
        {
            CurrentLink = null;
        }

        public void TriggerUp(object sender, ClickedEventArgs args)
        {

        }
    }
}

