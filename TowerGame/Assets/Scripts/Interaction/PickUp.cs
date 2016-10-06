using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Interaction
    {
        public class PickUp : MonoBehaviour
        {
            Holdable heldItem;
            float lastHeldTime;

            public float HoldCooldown;

            void OnCollisionEnter(Collision collision)
            {
                if (heldItem == null &&
                    collision.gameObject.tag == "Holdable" &&
                    Time.fixedTime > lastHeldTime + HoldCooldown)
                {
                    Holdable item = collision.gameObject.GetComponent<Holdable>();
                    if (item == null)
                    {
                        Debug.Log("Object tagged 'holdable' does not have a Holdable component");
                    }
                    else
                    {
                        item.PickUp(this.gameObject, GameObject.Find("Controller (right)"));
                        heldItem = item;
                        lastHeldTime = Time.fixedTime;
                    }
                }
            }

            void Update()
            {
                if (this.GetComponentInParent<SteamVR_TrackedObject>() != null)
                {
                    var index = GetComponentInParent<SteamVR_TrackedObject>().index;
                    if (SteamVR_Controller.Input((int)index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger) &&
                        heldItem != null)
                    {
                        heldItem.PutDown();
                        heldItem = null;
                    }
                }
            }
        }
    }
}