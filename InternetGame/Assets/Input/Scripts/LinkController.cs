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
            Debug.Log("Trigger down");
            GameObject LinkContainer = LinkFactory.CreateLink();
            LinkContainer.GetComponent<Link>().Initialize(InputManager.RightControllerObject.transform);

            CurrentLink = LinkContainer;
        }

        public void TriggerUp(object sender, ClickedEventArgs args)
        {
            Debug.Log("Trigger up");
            if (CurrentLink != null)
            {
                CurrentLink.GetComponent<Link>().End();
                CurrentLink = null;
            }
        }
    }
}

