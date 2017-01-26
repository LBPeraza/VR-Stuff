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
        public GameObject ArrowModel;
        public GameObject HandModel;
        public GameObject GrabModel;

        public LinkControllerState State;

        private GameObject CurrentLink;
        private GameObject LastLink;

        private PacketSource NearSource;
        private PacketSink NearSink;

        // Use this for initialization
        void Start()
        {
            State = LinkControllerState.Inactive;
            UpdateModel();

            InputManager.RightTriggerClicked += TriggerDown;
            InputManager.RightTriggerUnclicked += TriggerUp;
        }

        private void UpdateModel()
        {
            switch (State)
            {
                case LinkControllerState.Inactive:
                    ArrowModel.SetActive(true);
                    HandModel.SetActive(false);
                    GrabModel.SetActive(false);
                    break;
                case LinkControllerState.OverSource:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(true);
                    GrabModel.SetActive(false);
                    break;
                case LinkControllerState.DrawingLink:
                    ArrowModel.SetActive(false);
                    HandModel.SetActive(false);
                    GrabModel.SetActive(true);
                    break;
            }
        }


        private void AddLink()
        {
            GameObject LinkContainer = LinkFactory.CreateLink(NearSource);
            var linkSegment = LinkContainer.GetComponent<Link>();
            linkSegment.Initialize(InputManager.RightControllerObject.transform);

            // Listen for sever events.
            linkSegment.OnSever += LinkSegment_OnSever;

            CurrentLink = LinkContainer;

            State = LinkControllerState.DrawingLink;
            UpdateModel();
        }

        public void TriggerDown(object sender, ClickedEventArgs args)
        {
            if (CurrentLink == null && NearSource != null)
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
                UpdateModel();
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
                UpdateModel();
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
                    UpdateModel();
                }
            }
            else if (other.CompareTag("Sink"))
            {
                NearSink = null;
            }
        }

        private void LinkSegment_OnSever()
        {
            DestroyLink();
        }

        private void DestroyLink()
        {
            CurrentLink = null;
        }

        public void TriggerUp(object sender, ClickedEventArgs args)
        {
            //if (CurrentLink != null)
            //{
            //    var currentLinkComponent = CurrentLink.GetComponent<Link>();
            //    currentLinkComponent.End();

            //    LastLink = CurrentLink;

            //    DestroyLink();
            //}
        }
    }
}

