using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Slingshot
    {
        public class Slingshot : TowerGame.Interaction.Holdable
        {
            private GameObject slingHand;
            private GameObject sling;
            private bool aiming = false;

            public float aimThreshold = 1.0f;

            public override void Start()
            {
                base.Start();
                sling = this.transform.Find("sling").gameObject;
            }

            public override void PickUp(GameObject yokeHand, GameObject slingHand)
            {
                base.PickUp(yokeHand, slingHand);
                this.slingHand = slingHand;
            }

            void StartShot()
            {
                Transform slingTransform = slingHand.transform;
                if (!aiming && Vector3.Distance(slingTransform.position, transform.position) < aimThreshold)
                {
                    aiming = true;
                }
            }

            void Update()
            {

            }
        }
    }
}

