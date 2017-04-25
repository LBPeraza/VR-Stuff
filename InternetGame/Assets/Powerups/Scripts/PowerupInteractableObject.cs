using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace InternetGame
{
    public class PowerupInteractableObject : VRTK.VRTK_InteractableObject
    {
        private Powerup powerup;
        public void Initialize(Powerup p)
        {
            powerup = p;
        }

        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);

            powerup.OnGrabbed(e);
        }

        public override void ToggleSnapDropZone(VRTK_SnapDropZone snapDropZone, bool state)
        {
            base.ToggleSnapDropZone(snapDropZone, state);
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);

            powerup.OnDropped(hoveredOverSnapDropZone /* in drop zone */);
        }

        public void SetGrabbable(bool grabbable)
        {
            this.isGrabbable = grabbable;
            //gameObject.GetComponent<VRTK.Highlighters.VRTK_BaseHighlighter>().active = grabbable;
        }

        public void SetDroppable(ValidDropTypes dropType)
        {
            this.validDrop = dropType;
        }
    }
}
