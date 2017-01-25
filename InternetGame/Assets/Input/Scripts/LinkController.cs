using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkController : MonoBehaviour
    {
        private GameObject CurrentLink;

        // Use this for initialization
        void Start()
        {
            InputManager.RightTriggerClicked += TriggerDown;
            InputManager.RightTriggerUnclicked += TriggerUp;
        }

        public void TriggerDown(object sender, ClickedEventArgs args)
        {
            if (CurrentLink == null)
            {
                GameObject LinkContainer = LinkFactory.CreateLink();
                var linkSegment = LinkContainer.GetComponent<Link>();
                linkSegment.Initialize(InputManager.RightControllerObject.transform);
                // Listen for sever events.
                linkSegment.OnSever += LinkSegment_OnSever;

                CurrentLink = LinkContainer;
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
            if (CurrentLink != null)
            {
                var currentLinkComponent = CurrentLink.GetComponent<Link>();
                currentLinkComponent.End();

                DestroyLink();
            }
        }
    }
}

