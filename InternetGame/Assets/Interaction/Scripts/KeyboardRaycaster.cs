using CurvedVRKeyboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class KeyboardRaycaster : KeyboardComponent
    {
        //------Raycasting----
        [SerializeField, HideInInspector]
        private Transform raycastingSource;

        [SerializeField, HideInInspector]
        private GameObject target;

        private float rayLength;
        private Ray ray;
        private RaycastHit hit;
        private LayerMask layer;

        //---interactedKeys---
        private KeyboardStatus keyboardStatus;
        private KeyboardItem keyItemCurrent;

        private VRTK.VRTK_ControllerEvents input;

        [SerializeField, HideInInspector]
        private string clickInputName;

        private bool clickedBefore = false;
        private bool initialized = false;

        public void Initialize()
        {
            keyboardStatus = gameObject.GetComponent<KeyboardStatus>();
            int layerNumber = gameObject.layer;
            layer = 1 << layerNumber;

            var linkCursor = GameManager.GetInstance().Player.PrimaryCursor;
            if (linkCursor.Input != null)
            {
                input = linkCursor.Input;
            }

            initialized = true;
        }

        void Update()
        {
            if (initialized)
            {
                rayLength = Vector3.Distance(raycastingSource.position, target.transform.position) * 1.1f;
                RayCastKeyboard();
            }
        }

        /// <summary>
        /// Check if camera is pointing at any key. 
        /// If it does changes state of key
        /// </summary>
        private void RayCastKeyboard()
        {
            ray = new Ray(raycastingSource.position, raycastingSource.forward);
            if (Physics.Raycast(ray, out hit, rayLength, layer))
            { // If any key was hit
                KeyboardItem focusedKeyItem = hit.transform.gameObject.GetComponent<KeyboardItem>();
                if (focusedKeyItem != null)
                { // Hit may occur on item without script
                    ChangeCurrentKeyItem(focusedKeyItem);
                    keyItemCurrent.Hovering();
                    if (!clickedBefore && input && input.triggerClicked)
                    {// If key clicked
                        keyItemCurrent.Click();
                        keyboardStatus.HandleClick(keyItemCurrent);

                        clickedBefore = true;
                    }
                    else if (clickedBefore && input && !input.triggerClicked)
                    {
                        clickedBefore = false;
                    }
                }
            }
            else if (keyItemCurrent != null)
            {// If no target hit and lost focus on item
                ChangeCurrentKeyItem(null);
            }
        }

        private void ChangeCurrentKeyItem(KeyboardItem key)
        {
            if (keyItemCurrent != null)
            {
                keyItemCurrent.StopHovering();
            }
            keyItemCurrent = key;
        }

        //---Setters---
        public void SetRayLength(float rayLength)
        {
            this.rayLength = rayLength;
        }

        public void SetRaycastingTransform(Transform raycastingSource)
        {
            this.raycastingSource = raycastingSource;
        }

        public void SetClickButton(string clickInputName)
        {
            this.clickInputName = clickInputName;
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }
    }
}

