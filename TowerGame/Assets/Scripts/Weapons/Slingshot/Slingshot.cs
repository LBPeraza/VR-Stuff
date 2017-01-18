using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Weapons
    {
        public class Slingshot : TowerGame.Interaction.Holdable
        {
			private GameObject yokeHand;
            private GameObject slingHand;
            private bool yokeHandIsLeftHand = false;
            private GameObject sling;
			private Transform slingOrigin;
            private bool aiming = false;
			private Rigidbody slingRB;

			private Transform slingTransform;

            public float aimThreshold = 0.3f;
			public float slingVelFactor = 200f;
			public float slingForceFactor = 20000f;

            public override void Start()
            {
                base.Start();
				slingTransform = transform.Find ("SlingPos").transform;
				sling = transform.Find ("Sling").gameObject;
				slingRB = sling.GetComponent<Rigidbody> ();
                // sling = this.transform.Find ("sling").gameObject;
                // slingOrigin = this.transform.Find ("SlingOrigin");        
            }

            private void TriggerClicked()
            {
                Debug.Log("Trigger down");
                StartShot();
            }

            private void TriggerReleased()
            {
                Debug.Log("Trigger up");
                Shoot();
            }

            public override bool PickUp(GameObject leftHand, GameObject rightHand, bool leftHandIsPickingUp)
            {
                var success = base.PickUp(leftHand, rightHand, leftHandIsPickingUp);

                if (success)
                {
                    yokeHand = leftHandIsPickingUp ? leftHand : rightHand;
                    slingHand = leftHandIsPickingUp ? rightHand : leftHand;
                    yokeHandIsLeftHand = leftHandIsPickingUp;

                    if (leftHandIsPickingUp)
                    {
                        InputManager.OnRightTriggerClick += TriggerClicked;
                        InputManager.OnRightTriggerReleased += TriggerReleased;
                    }
                    else
                    {
                        InputManager.OnLeftTriggerClick += TriggerClicked;
                        InputManager.OnLeftTriggerReleased += TriggerReleased;
                    }
                }

                return success;
            }

            public override bool PutDown()
            {
                if (base.PutDown())
                {
                    if (yokeHandIsLeftHand)
                    {
                        InputManager.OnRightTriggerClick -= TriggerClicked;
                        InputManager.OnRightTriggerReleased -= TriggerReleased;
                    }
                    else
                    {
                        InputManager.OnLeftTriggerClick -= TriggerClicked;
                        InputManager.OnLeftTriggerReleased -= TriggerReleased;
                    }
                    return true;
                }
                return false;
            }

            public bool StartShot()
            {
                Transform slingTransform = slingHand.transform;
                if (!aiming && Vector3.Distance(slingTransform.position, transform.position) < aimThreshold)
                {
                    aiming = true;
                    return true;
                }
                return false;
            }

			public override void Update() {
				base.Update ();
				sling.transform.rotation = slingTransform.rotation;

				Vector3 posDelta;
				if (aiming) {
					posDelta = slingHand.transform.position - sling.transform.position;
					slingRB.velocity = posDelta * Time.fixedDeltaTime * slingVelFactor;
				} else {
					//sling.transform.position = slingTransform.position;
					posDelta = slingTransform.position - sling.transform.position;
					slingRB.AddForce (
						posDelta * Time.fixedDeltaTime * slingForceFactor
					);
					//sling.transform.rotation = slingTransform.rotation;
				}
			}

			public bool Shoot() {
                if (aiming)
                {
                    aiming = false;
                    return true;
                }
                return false;
			}
        }
    }
}

