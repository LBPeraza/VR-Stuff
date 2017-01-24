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
            GameObject LinkContainer = LinkFactory.CreateLink();
            LinkContainer.GetComponent<Link>().Initialize(InputManager.RightControllerObject.transform);

            CurrentLink = LinkContainer;
        }

        public void TriggerUp(object sender, ClickedEventArgs args)
        {
            if (CurrentLink != null)
            {
                var currentLinkComponent = CurrentLink.GetComponent<Link>();
                currentLinkComponent.End();
                Destroy(currentLinkComponent);
                CurrentLink = null;
            }
        }
    }
}

