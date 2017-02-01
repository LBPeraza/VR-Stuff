using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Interactable : MonoBehaviour
    {
        public virtual CursorEventArgs OnEnter()
        {
            return cursorEventArgs;
        }
        public virtual CursorEventArgs OnExit()
        {
            return cursorEventArgs;
        }
        public virtual CursorEventArgs OnClick()
        {
            return cursorEventArgs;
        }
        public virtual CursorEventArgs OnUnclick()
        {
            return cursorEventArgs;
        }

        private bool? leftCursorEntered;
        private int? trackedColliderInstanceId;
        private Cursor trackedCursor;

        protected CursorEventArgs cursorEventArgs;

        private void OnTriggerClicked(object sender, ClickedEventArgs args)
        {
            // Trigger was clicked while entered.
            var cursorArgs = OnClick();
            trackedCursor.OnGrab(cursorArgs);
        }

        private void OnTriggerUnclicked(object sender, ClickedEventArgs args)
        {
            // Trigger was released while entered.
            var cursorArgs = OnUnclick();
            trackedCursor.OnDrop(cursorArgs);
        }

        private bool? IsLeftCursor(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                return other.transform.parent.name == "LeftCursor";
            }

            return null;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetInstanceID() == trackedColliderInstanceId.Value)
            {
                // The cursor we were tracking exited.
                if (leftCursorEntered.Value)
                {
                    InputManager.LeftTriggerClicked -= OnTriggerClicked;
                    InputManager.LeftTriggerUnclicked -= OnTriggerUnclicked;
                }
                else
                {
                    InputManager.RightTriggerClicked -= OnTriggerClicked;
                    InputManager.RightTriggerUnclicked -= OnTriggerUnclicked;
                }

                var cursorArgs = OnExit();
                trackedCursor.OnExit(cursorArgs);

                leftCursorEntered = null;
                trackedColliderInstanceId = null;
                trackedCursor = null;
            }                
        }

        private void OnTriggerEnter(Collider other)
        {
            if (trackedColliderInstanceId == null)
            {
                // Only fire event if not already tracking a cursor.
                bool? isLeftCursor = IsLeftCursor(other);

                if (isLeftCursor != null)
                {
                    leftCursorEntered = isLeftCursor.Value;
                    trackedColliderInstanceId = other.GetInstanceID();
                    trackedCursor = other.transform.parent.GetComponent<Cursor>();

                    // This helps the Cursor track where events are coming from.
                    cursorEventArgs.senderId = trackedColliderInstanceId.Value;

                    // Set up trigger listener
                    if (leftCursorEntered.Value)
                    {
                        InputManager.LeftTriggerClicked += OnTriggerClicked;
                        InputManager.LeftTriggerUnclicked += OnTriggerUnclicked;
                    }
                    else
                    {
                        InputManager.RightTriggerClicked += OnTriggerClicked;
                        InputManager.RightTriggerUnclicked += OnTriggerUnclicked;
                    }

                    var cursorArgs = OnEnter();
                    trackedCursor.OnEnter(cursorArgs);
                }
            }
        }

    }

}

