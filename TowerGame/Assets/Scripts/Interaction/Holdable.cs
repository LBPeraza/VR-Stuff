using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Interaction
    {
        public class Holdable : MonoBehaviour
        {
			private Rigidbody rigidbody;
            private GameObject holder;
            private bool isHeld;
			private Transform interactionPoint;

            public Transform HeldTransform;
            public float grabThreshold = 1.0f;

			public float velocityFactor = 4000f;
			public float rotationFactor = 40f;

			public GameObject GetHolder() {
				return holder;
			}

            public virtual void Start()
            {
                holder = null;
                isHeld = false;
                transform.tag = "Holdable";
				rigidbody = GetComponent<Rigidbody> ();
				interactionPoint = new GameObject ().transform;
            }

            public virtual bool PickUp(GameObject leftHand, GameObject rightHand, bool leftHandIsPickingUp)
            {
                GameObject holdingHand = leftHandIsPickingUp ? leftHand : rightHand;
                Transform holdingHandTransform = holdingHand.transform;
                if (!isHeld && Vector3.Distance(holdingHandTransform.position, transform.position) < grabThreshold)
                {
					isHeld = true;
					holder = holdingHand;
                    interactionPoint.position = holdingHand.transform.position;
                    interactionPoint.rotation = holdingHand.transform.rotation;
					interactionPoint.SetParent (transform, true);
                    return true;
					/*
                    isHeld = true;
                    this.GetComponent<Rigidbody>().isKinematic = true;
                    this.transform.SetParent(holdingHand.transform);

                    if (HeldTransform != null)
                    {
                        this.transform.localPosition = HeldTransform.position;
                        this.transform.localRotation = HeldTransform.rotation;
                    } else
                    {
                        this.transform.localPosition = Vector3.zero;
                        this.transform.localRotation = Quaternion.identity;
                    }


                    return true;
                    */
                }

                return false;
            }

            /// <summary>
            /// Puts down the item.
            /// </summary>
            public virtual bool PutDown()
            {
                if (isHeld)
                {
                    holder = null;
                    isHeld = false;
                    interactionPoint.SetParent(null);
                    return true;
                }
                return false;
                //this.GetComponent<Rigidbody>().isKinematic = false;
                //this.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
            }

            public bool IsHeld()
            {
                return isHeld;
            }

			public virtual void Update() {
				if (IsHeld () && holder) {
					Vector3 posDelta = holder.transform.position - interactionPoint.position;
					this.rigidbody.velocity = posDelta * Time.fixedDeltaTime * velocityFactor;

					Quaternion rotationDelta = holder.transform.rotation * Quaternion.Inverse (interactionPoint.rotation);
					float angle; Vector3 axis;
					rotationDelta.ToAngleAxis (out angle, out axis);
					if (angle > 180)
						angle -= 360;

					this.rigidbody.angularVelocity =
						Time.fixedDeltaTime * angle * axis * rotationFactor;
				}
			}
        }
    }
}

