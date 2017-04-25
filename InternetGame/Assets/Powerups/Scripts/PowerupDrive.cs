using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PowerupDrive : MonoBehaviour
    {
        protected VRTK.VRTK_SnapDropZone[] DropZones;

        public void Initialize()
        {
            DropZones = transform.GetComponentsInChildren<VRTK.VRTK_SnapDropZone>();
            foreach (var dz in DropZones)
            {
                dz.ObjectSnappedToDropZone += PowerupSnappedToDropZone;
            }
        }

        private void PowerupSnappedToDropZone(object sender, VRTK.SnapDropZoneEventArgs e)
        {
            var powerup = e.snappedObject.GetComponentInParent<Powerup>();
        }
    }
}
