using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Slingshot
    {
        public class Slingshot : TowerGame.Interaction.Holdable
        {
			private GameObject yokeHand;
            private GameObject slingHand;
            private GameObject sling;
			private Transform slingOrigin;
            private bool aiming = false;
			private Rigidbody slingRB;

			private Transform slingTransform;

            public float aimThreshold = 1.0f;
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

            public override void PickUp(GameObject leftHand, GameObject rightHand, bool leftHandIsPickingUp)
            {
                base.PickUp(leftHand, rightHand, leftHandIsPickingUp);
				yokeHand = leftHandIsPickingUp ? leftHand : rightHand;
                slingHand = leftHandIsPickingUp ? rightHand : leftHand;
            }

            public void StartShot()
            {
                Transform slingTransform = slingHand.transform;
                if (!aiming && Vector3.Distance(slingTransform.position, transform.position) < aimThreshold)
                {
                    aiming = true;
                }
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

			public void Shoot() {
				if (aiming)
					aiming = false;
			}
        }
    }
}

