using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Interaction
    {
        public class Holdable : MonoBehaviour
        {
            private GameObject holder;
            private bool isHeld;

            public Transform HeldTransform;
            public float grabThreshold = 1.0f;

            public virtual void Start()
            {
                holder = null;
                isHeld = false;
                this.transform.tag = "Holdable";
            }

            public virtual bool PickUp(GameObject leftHand, GameObject rightHand, bool leftHandIsPickingUp)
            {
                GameObject holdingHand = leftHandIsPickingUp ? leftHand : rightHand;
                Transform holdingHandTransform = holdingHand.transform;
                if (!isHeld && Vector3.Distance(holdingHandTransform.position, transform.position) < grabThreshold)
                {
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
                }

                return false;
            }

            /// <summary>
            /// Puts down the item.
            /// </summary>
            /// <param name="velocity"> The velocity with which the item is being dropped. </param>
            public void PutDown(Vector3 velocity)
            {
                this.transform.SetParent(null);
                holder = null;
                isHeld = false;
                this.GetComponent<Rigidbody>().isKinematic = false;
                this.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
            }

            public bool IsHeld()
            {
                return isHeld;
            }
        }
    }
}

