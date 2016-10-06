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

            private float triggerDownMemory = 0.4f;
            private float lastTriggerDownTime;
            private Vector3 lastPosition;

            void Start()
            {
                if (this.transform.parent.name == "Controller (left)")
                {
                    leftHandInteractor = this.gameObject;
                    rightHandInteractor = GameObject.Find("Controller (right)/Interactor");
                    IsLeftHand = true;
                } else
                {
                    rightHandInteractor = this.gameObject;
                    leftHandInteractor = GameObject.Find("Controller (left)/Interactor");
                    IsLeftHand = false;
                }

                if (leftHandInteractor == null || rightHandInteractor == null)
                {
                    Debug.LogWarning("Failed to properly initialize both interactors.");
                }

                room = GameObject.Find("[CameraRig]");

                if (room == null)
                {
                    Debug.LogError("Could not find Camera Rig in scene.");
                }
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

            void OnCollisionEnter(Collision collision)
            {
                if (heldItem == null &&
                    collision.gameObject.tag == "Holdable" &&
                    Time.fixedTime > lastHeldTime + HoldCooldown &&
                    ShouldPickUpItem(PickUpPolicy, lastTriggerDownTime))
                {
                    Holdable item = collision.gameObject.GetComponent<Holdable>();
                    if (item == null)
                    {
                        Debug.LogWarning("Object tagged 'holdable' does not have a Holdable component");
                    }
                    else
                    {
                        item.PickUp(this.leftHandInteractor, this.rightHandInteractor, IsLeftHand);
                        heldItem = item;
                        lastHeldTime = Time.fixedTime;
                    }
                }
            }

            void Update()
            {
                if (trackedObject != null || CheckTrackedObjectOnParent())
                {
                    // Store trigger down so that we can use this in the OnCollisionEnter function to
                    // implement "pick up with trigger" policy.
                    if (SteamVR_Controller.Input((int)trackedObject.index)
                        .GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                    {
                        lastTriggerDownTime = Time.fixedTime;
                    }

                    if (heldItem != null &&
                        Time.fixedTime > lastHeldTime + DropCooldown &&
                        ShouldPutDownItem(PutDownPolicy, this.transform, this.room.transform, 
                        lastTriggerDownTime))
                    {
                        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
                        heldItem.PutDown(velocity);
                        heldItem = null;
                        lastHeldTime = Time.fixedTime;
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