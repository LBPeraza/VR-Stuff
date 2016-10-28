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

			public float velocityFactor = 2000f;
			public float rotationFactor = 20f;

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
					interactionPoint.position = Vector3.zero;
					interactionPoint.rotation = Quaternion.identity;
					interactionPoint.SetParent (transform, true);
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
            /// <param name="velocity"> The velocity with which the item is being dropped. </param>
            public void PutDown(Vector3 velocity)
            {
                holder = null;
                isHeld = false;
				interactionPoint.SetParent (null);
                //this.GetComponent<Rigidbody>().isKinematic = false;
                //this.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
            }

            public bool IsHeld()
            {
                return isHeld;
            }

			public virtual void Update() {
				if (IsHeld () && holder) {
					Vector3 posDelta = holder.transform.position - transform.position;
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

