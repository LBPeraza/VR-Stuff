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

            public virtual void PickUp(GameObject hand, GameObject otherHand)
            {
                Transform handTransform = hand.transform;
                if (!isHeld && Vector3.Distance(handTransform.position, transform.position) < grabThreshold)
                {
                    holder = hand;
                    isHeld = true;
                    this.GetComponent<Rigidbody>().isKinematic = true;
                    this.transform.SetParent(holder.transform);

                    if (HeldTransform != null)
                    {
                        this.transform.localPosition = HeldTransform.position;
                        this.transform.localRotation = HeldTransform.rotation;
                    } else
                    {
                        this.transform.localPosition = Vector3.zero;
                        this.transform.localRotation = Quaternion.identity;
                    }
                }
            }

            public void PutDown()
            {
                this.transform.SetParent(null);
                holder = null;
                isHeld = false;
                this.GetComponent<Rigidbody>().isKinematic = false;
            }

            public bool IsHeld()
            {
                return isHeld;
            }
        }
    }
}

