using UnityEngine;
using System.Collections;

namespace InternetGame
{
    public class InputManager : MonoBehaviour
    {
        public static SteamVR_Controller.Device LeftDevice;
        public static SteamVR_Controller.Device RightDevice;

        public static SteamVR_TrackedController LeftController;
        public static SteamVR_TrackedController RightController;

        public static GameObject LeftControllerObject;
        public static GameObject RightControllerObject;

        public bool IsInitialized = false;

        public static event ClickedEventHandler LeftMenuButtonClicked;
        public static event ClickedEventHandler LeftMenuButtonUnclicked;
        public static event ClickedEventHandler LeftTriggerClicked;
        public static event ClickedEventHandler LeftTriggerUnclicked;
        public static event ClickedEventHandler LeftSteamClicked;
        public static event ClickedEventHandler LeftPadClicked;
        public static event ClickedEventHandler LeftPadUnclicked;
        public static event ClickedEventHandler LeftPadTouched;
        public static event ClickedEventHandler LeftPadUntouched;
        public static event ClickedEventHandler LeftGripped;
        public static event ClickedEventHandler LeftUngripped;

        public static event ClickedEventHandler RightMenuButtonClicked;
        public static event ClickedEventHandler RightMenuButtonUnclicked;
        public static event ClickedEventHandler RightTriggerClicked;
        public static event ClickedEventHandler RightTriggerUnclicked;
        public static event ClickedEventHandler RightSteamClicked;
        public static event ClickedEventHandler RightPadClicked;
        public static event ClickedEventHandler RightPadUnclicked;
        public static event ClickedEventHandler RightPadTouched;
        public static event ClickedEventHandler RightPadUntouched;
        public static event ClickedEventHandler RightGripped;
        public static event ClickedEventHandler RightUngripped;

        public static bool LeftControllerTracking
        {
            get
            {
                return (LeftDevice != null && LeftDevice.connected);
            }
        }
        public static bool RightControllerTracking
        {
            get
            {
                return (RightDevice != null && RightDevice.connected);
            }
        }

        private bool registeredLeftController = false;
        private bool registeredRightController = false;

        private bool InitializeControllers()
        {
            return InitializeLeftController() & InitializeRightController();
        }

        private bool InitializeLeftController()
        {
            var leftController = GameObject.Find("/[CameraRig]/Controller (left)");
            
            if (leftController != null)
            {
                LeftControllerObject = leftController;
                var leftDevice = leftController.GetComponent<SteamVR_TrackedObject>();
                if (leftDevice)
                {
                    LeftDevice = SteamVR_Controller.Input((int)leftDevice.index);
                }
                LeftController = leftController.GetComponent<SteamVR_TrackedController>();

                if (LeftController != null && !registeredLeftController)
                {
                    RegisterLeftController();
                    registeredLeftController = true;
                }
            }

            return LeftControllerTracking;
        }

        private bool InitializeRightController()
        {
            var rightController = GameObject.Find("/[CameraRig]/Controller (right)");

            if (rightController != null)
            {
                RightControllerObject = rightController;
                var rightDevice = rightController.GetComponent<SteamVR_TrackedObject>();
                if (rightDevice)
                {
                    RightDevice = SteamVR_Controller.Input((int)rightDevice.index);
                }
                RightController = rightController.GetComponent<SteamVR_TrackedController>();

                if (RightController != null && !registeredRightController)
                {
                    RegisterRightController();
                    registeredRightController = true;
                }
            }

            return RightControllerTracking;
        }

        private void RegisterLeftController()
        {
            if (LeftController)
            {
                LeftController.MenuButtonClicked += Handle_LeftMenuButtonClicked;
                LeftController.MenuButtonUnclicked += Handle_LeftMenuButtonUnclicked;
                LeftController.TriggerClicked += Handle_LeftTriggerClicked;
                LeftController.TriggerUnclicked += Handle_LeftTriggerUnclicked;
                LeftController.SteamClicked += Handle_LeftSteamClicked;
                LeftController.PadClicked += Handle_LeftPadClicked;
                LeftController.PadUnclicked += Handle_LeftPadUnclicked;
                LeftController.PadTouched += Handle_LeftPadTouched;
                LeftController.PadUntouched += Handle_LeftPadUntouched;
                LeftController.Gripped += Handle_LeftGripped;
                LeftController.Ungripped += Handle_LeftUngripped;
            }
        }

        private void RegisterRightController()
        {
            if (RightController)
            {
                RightController.MenuButtonClicked += Handle_RightMenuButtonClicked;
                RightController.MenuButtonUnclicked += Handle_RightMenuButtonUnclicked;
                RightController.TriggerClicked += Handle_RightTriggerClicked;
                RightController.TriggerUnclicked += Handle_RightTriggerUnclicked;
                RightController.SteamClicked += Handle_RightSteamClicked;
                RightController.PadClicked += Handle_RightPadClicked;
                RightController.PadUnclicked += Handle_RightPadUnclicked;
                RightController.PadTouched += Handle_RightPadTouched;
                RightController.PadUntouched += Handle_RightPadUntouched;
                RightController.Gripped += Handle_RightGripped;
                RightController.Ungripped += Handle_RightUngripped;
            }
        }

        // Use this for initialization
        public void Initialize()
        {
            InitializeControllers();

            IsInitialized = true;
        }

        void Start()
        {
            if (!IsInitialized)
            {
                Initialize();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!LeftControllerTracking)
            {
                InitializeLeftController();
            }
            if (!RightControllerTracking)
            {
                InitializeRightController();
            }
        }

        public static void RumbleController(bool rightController, ushort length)
        {
            length = (ushort) Mathf.Clamp(length, 0, 3999);
            if (rightController)
            {
                if (RightDevice != null)
                {
                    RightDevice.TriggerHapticPulse(length);
                }
            }
            else
            {
                if (LeftDevice != null)
                {
                    LeftDevice.TriggerHapticPulse(length);
                }
            } 
        }

        #region Left Controller Event Handlers
        private void Handle_LeftMenuButtonClicked(object sender, ClickedEventArgs args)
        {
            if (LeftMenuButtonClicked != null)
            {
                LeftMenuButtonClicked.Invoke(sender, args);
            }  
        }
        private void Handle_LeftMenuButtonUnclicked(object sender, ClickedEventArgs args)
        {
            if (LeftMenuButtonUnclicked != null)
            {
                LeftMenuButtonUnclicked.Invoke(sender, args);
            }
        }
        private void Handle_LeftTriggerClicked(object sender, ClickedEventArgs args)
        {
            if (LeftTriggerClicked != null)
            {
                LeftTriggerClicked.Invoke(sender, args);
            }
        }
        private void Handle_LeftTriggerUnclicked(object sender, ClickedEventArgs args)
        {
            if (LeftTriggerUnclicked != null)
            {
                LeftTriggerUnclicked.Invoke(sender, args);
            }
        }
        private void Handle_LeftSteamClicked(object sender, ClickedEventArgs args)
        {
            if (LeftSteamClicked != null)
            {
                LeftSteamClicked.Invoke(sender, args);
            }
        }
        private void Handle_LeftPadClicked(object sender, ClickedEventArgs args)
        {
            if (LeftPadClicked != null)
            {
                LeftPadClicked.Invoke(sender, args);
            }
        }
        private void Handle_LeftPadUnclicked(object sender, ClickedEventArgs args)
        {
            if (LeftPadUnclicked != null)
            {
                LeftPadUnclicked.Invoke(sender, args);
            }
        }
        private void Handle_LeftPadTouched(object sender, ClickedEventArgs args)
        {
            if (LeftPadTouched != null)
            {
                LeftPadTouched.Invoke(sender, args);
            }
        }
        private void Handle_LeftPadUntouched(object sender, ClickedEventArgs args)
        {
            if (LeftPadUntouched != null)
            {
                LeftPadUntouched.Invoke(sender, args);
            }
        }
        private void Handle_LeftGripped(object sender, ClickedEventArgs args)
        {
            if (LeftGripped != null)
            {
                LeftGripped.Invoke(sender, args);
            }
        }
        private void Handle_LeftUngripped(object sender, ClickedEventArgs args)
        {
            if (LeftUngripped != null)
            {
                LeftUngripped.Invoke(sender, args);
            }
        }
        #endregion

        #region Right Controller Event Handlers
        private void Handle_RightMenuButtonClicked(object sender, ClickedEventArgs args)
        {
            if (RightMenuButtonClicked != null)
            {
                RightMenuButtonClicked.Invoke(sender, args);
            }
        }
        private void Handle_RightMenuButtonUnclicked(object sender, ClickedEventArgs args)
        {
            if (RightMenuButtonUnclicked != null)
            {
                RightMenuButtonUnclicked.Invoke(sender, args);
            }
        }
        private void Handle_RightTriggerClicked(object sender, ClickedEventArgs args)
        {
            if (RightTriggerClicked != null)
            {
                RightTriggerClicked.Invoke(sender, args);
            }
        }
        private void Handle_RightTriggerUnclicked(object sender, ClickedEventArgs args)
        {
            if (RightTriggerUnclicked != null)
            {
                RightTriggerUnclicked.Invoke(sender, args);
            }
        }
        private void Handle_RightSteamClicked(object sender, ClickedEventArgs args)
        {
            if (RightSteamClicked != null)
            {
                RightSteamClicked.Invoke(sender, args);
            }
        }
        private void Handle_RightPadClicked(object sender, ClickedEventArgs args)
        {
            if (RightPadClicked != null)
            {
                RightPadClicked.Invoke(sender, args);
            }
        }
        private void Handle_RightPadUnclicked(object sender, ClickedEventArgs args)
        {
            if (RightPadUnclicked != null)
            {
                RightPadUnclicked.Invoke(sender, args);
            }
        }
        private void Handle_RightPadTouched(object sender, ClickedEventArgs args)
        {
            if (RightPadTouched != null)
            {
                RightPadTouched.Invoke(sender, args);
            }
        }
        private void Handle_RightPadUntouched(object sender, ClickedEventArgs args)
        {
            if (RightPadUntouched != null)
            {
                RightPadUntouched.Invoke(sender, args);
            }
        }
        private void Handle_RightGripped(object sender, ClickedEventArgs args)
        {
            if (RightGripped != null)
            {
                RightGripped.Invoke(sender, args);
            }
        }
        private void Handle_RightUngripped(object sender, ClickedEventArgs args)
        {
            if (RightUngripped != null)
            {
                RightUngripped.Invoke(sender, args);
            }
        }
        #endregion
    }
}