using UnityEngine;
using System.Collections;
using System;

namespace TowerGame
{
    namespace Interaction
    {
        public enum PickUpPolicy
        {
            JustCollision = 0,
            CollisionAndTrigger = 1
        }

        public enum PutDownPolicy
        {
            CloseToGround = 0,
            Trigger = 1
        }

        public class Interactor : MonoBehaviour
        {
            public bool IsLeftHand;

            public float HoldCooldown;
            public float DropCooldown;
            public float PutDownFloorDistanceThreshold;

            public PickUpPolicy PickUpPolicy;
            public PutDownPolicy PutDownPolicy;

            protected GameObject leftHandInteractor;
            protected GameObject rightHandInteractor;
            protected GameObject room; 

            protected SteamVR_TrackedObject trackedObject;
            protected Holdable heldItem;
            protected float lastHeldTime;
            protected float lastTimeFarFromGround;
            protected Holdable lastCollidedItem;
            protected float lastCollidedItemTime;

            private float collidedItemMemory = 0.7f; // seconds
            private float triggerDownMemory = 0.7f; // seconds
            private float lastTriggerDownTime;
            private Vector3 lastPosition;

            public void Initialize(GameObject leftHand, GameObject rightHand, bool isLeftHand, GameObject room)
            {
                this.leftHandInteractor = leftHand;
                this.rightHandInteractor = rightHand;
                this.IsLeftHand = isLeftHand;
                this.room = room;

                if (isLeftHand)
                {
                    InputManager.OnLeftTriggerClick += OnTriggerClick;
                } else
                {
                    InputManager.OnRightTriggerClick += OnTriggerClick;
                }
            }

            private void OnTriggerClick()
            {
                lastTriggerDownTime = Time.fixedTime;
            }

            /// <summary>
            /// Looks for the "SteamVR_TrackedObject" component on the parent element and sets
            /// a local reference to it if one is found.
            /// </summary>
            /// <returns> Whether the component was found on the parent. </returns>
            protected bool CheckTrackedObjectOnParent()
            {
                var candidate = this.GetComponentInParent<SteamVR_TrackedObject>();
                if (candidate != null)
                {
                    trackedObject = candidate;
                    return true;
                }

                return false;
            }

            void OnTriggerEnter(Collider collider)
            {
                // TODO: make sure that the colliders on the interactor dont collide with themselves.
                if (collider.gameObject.tag == "Holdable")
                {
                    Holdable item = collider.gameObject.GetComponent<Holdable>();
                    if (item == null)
                    {
                        Debug.LogWarning("Object tagged 'holdable' does not have a Holdable component");
                    }
                    else
                    {
                        lastCollidedItemTime = Time.fixedTime;
                        lastCollidedItem = item;
                    }
                }
            }

            private void Vibrate()
            {
                InputManager.LeftController.TriggerHapticPulse(2000);
            }

            private void PickUpItem(Holdable item)
            {
                if (item.PickUp(this.leftHandInteractor, this.rightHandInteractor, IsLeftHand))
                {
                    heldItem = item;
                    lastHeldTime = Time.fixedTime;
                }

                lastCollidedItem = null;
                lastCollidedItemTime = 0;

                Vibrate();
            }

            private void PutDownItem(Holdable item, Vector3 currentVelocity)
            {
                item.PutDown();
                heldItem = null;
                lastHeldTime = Time.fixedTime;

                Vibrate();
            }

            void Update()
            {
                // Pick Up logic
                if (heldItem == null &&
                    lastCollidedItem != null && Time.fixedTime < lastCollidedItemTime + collidedItemMemory &&
                    Time.fixedTime > lastHeldTime + HoldCooldown &&
                    ShouldPickUpItem(PickUpPolicy, lastTriggerDownTime))
                {
                    PickUpItem(lastCollidedItem);
                }
                
                if (trackedObject != null || CheckTrackedObjectOnParent())
                {
                    // Changes pick-up/put-down policy on touchpad press.
                    //if (controller.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
                    //{
                    //    var axis = controller.GetAxis();
                    //    Debug.Log(axis);
                    //    if (axis.x < -0.5)
                    //    {
                    //        PickUpPolicy = PickUpPolicy == PickUpPolicy.JustCollision ? 
                    //            PickUpPolicy.CollisionAndTrigger : PickUpPolicy.JustCollision;  
                    //    }
                    //    if (axis.x > 0.5)
                    //    {
                    //        PutDownPolicy = PutDownPolicy == PutDownPolicy.CloseToGround ?
                    //            PutDownPolicy.Trigger : PutDownPolicy.CloseToGround;
                    //    }
                    //}
                    
                    // Put down logic.
                    if (heldItem != null &&
                        Time.fixedTime > lastHeldTime + DropCooldown &&
                        ShouldPutDownItem(PutDownPolicy, this.transform, this.room.transform, lastTriggerDownTime))
                    {
                        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
                        PutDownItem(heldItem, velocity);
                    }
                }
                
                lastPosition = transform.position;
            }

            /// <summary>
            /// Implements the decision policy on how to pick up items.
            /// </summary>
            /// <param name="policy"> The pick-up policy to implement.</param>
            /// <param name="lastTriggerDownTime"> When the last time a trigger pull was detected. </param>
            /// <returns> Whether the item should be picked up. </returns>
            protected bool ShouldPickUpItem(PickUpPolicy policy, float lastTriggerDownTime)
            {
                switch (policy)
                {
                    case PickUpPolicy.JustCollision:
                        return true;
                    case PickUpPolicy.CollisionAndTrigger:
                        return Time.fixedTime < lastTriggerDownTime + triggerDownMemory;
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Implements the decision policy on how to put down items.
            /// </summary>
            /// <param name="policy"> The put-down policy to implement.</param>
            /// <param name="triggerDown"> Whether the trigger was recently depressed. </param>
            /// <param name="floorPosition"> The transform of the room's floor. </param>
            /// <param name="lastTriggerDownTime"> When we last detected a trigger pull. </param>
            /// <returns> Whether the item should be picked up. </returns>
            protected bool ShouldPutDownItem(PutDownPolicy policy, Transform handPosition, 
                Transform floorPosition, float lastTriggerDownTime)
            {
                switch (policy)
                {
                    case PutDownPolicy.CloseToGround:
                        var distanceFromGround = Math.Abs(handPosition.TransformPoint(0, 0, 0).y - 
                            floorPosition.TransformPoint(0, 0, 0).y);
                        if (distanceFromGround > PutDownFloorDistanceThreshold)
                        {
                            lastTimeFarFromGround = Time.fixedTime;
                            return false;
                        }
                        // If we are close to ground, only drop item if we went far from ground before
                        // becoming close to ground again. Ie., we went up and then down.
                        return (lastTimeFarFromGround > lastHeldTime);
                    case PutDownPolicy.Trigger:
                        return Time.fixedTime < lastTriggerDownTime + triggerDownMemory;
                    default:
                        return false;
                }
            }
        }
    }
}