using UnityEngine;
using System.Collections;

namespace TowerGame
{
    public class InputManager : MonoBehaviour
    {
        public static SteamVR_Controller.Device LeftController;
        public static SteamVR_Controller.Device RightController;

        public bool LeftControllerTracking = false;
        public bool RightControllerTracking = false;

        public delegate void TriggerClicked();
        public static event TriggerClicked OnRightTriggerClick;
        public static event TriggerClicked OnLeftTriggerClick;

        public delegate void TriggerReleased();
        public static event TriggerReleased OnRightTriggerReleased;
        public static event TriggerReleased OnLeftTriggerReleased;

        private bool InitializeControllers()
        {
            var leftController = GameObject.Find("Controller (left)");
            var rightController = GameObject.Find("Controller (right)");

            if (leftController != null)
            {
                var leftDevice = leftController.GetComponent<SteamVR_TrackedObject>();
                if (leftDevice)
                {
                    LeftControllerTracking = true;
                    LeftController = SteamVR_Controller.Input((int)leftDevice.index);
                }
            }

            if (rightController != null)
            {
                var rightDevice = rightController.GetComponent<SteamVR_TrackedObject>();
                if (rightDevice)
                {
                    RightControllerTracking = true;
                    RightController = SteamVR_Controller.Input((int)rightDevice.index);
                }
            }

            return LeftControllerTracking && RightControllerTracking;
        }

        // Use this for initialization
        void Start()
        {
           InitializeControllers();
        }

        protected void PollTriggers()
        {
            if (LeftControllerTracking)
            {
                var leftTriggerIsDown = LeftController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
                var leftTriggerIsUp = LeftController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
                if (leftTriggerIsDown)
                {
                    if (OnLeftTriggerClick != null)
                    {
                        OnLeftTriggerClick.Invoke();
                    }
                    OnLeftTriggerClick.Invoke();
                } else if (leftTriggerIsUp)
                {
                    if (OnLeftTriggerReleased != null)
                    {
                        OnLeftTriggerReleased.Invoke();
                    }
                }
            }

            if (RightControllerTracking) { 
                var rightTriggerIsDown = RightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
                var rightTriggerIsUp = RightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);

                if (rightTriggerIsDown)
                {
                    if (OnRightTriggerClick != null)
                    {
                        OnRightTriggerClick.Invoke();
                    }
                } else if (rightTriggerIsUp)
                {
                    if (OnRightTriggerReleased != null)
                    {
                        OnRightTriggerReleased.Invoke();
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!(LeftControllerTracking && RightControllerTracking) || LeftController == null || RightController == null)
            {
                InitializeControllers();
            }
            else
            {
                PollTriggers();
            }
        }
    }
}