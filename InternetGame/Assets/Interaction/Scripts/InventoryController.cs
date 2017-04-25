using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class InventoryController : MonoBehaviour
    {
        public PowerupInventory PowerupInventory;

        protected Player player;
        protected Cursor cursor;

        public void Initialize(Player p)
        {
            player = p;

            // Create a powerup inventory and initialize it.
            GameObject inventoryContainer = new GameObject("[PowerupInventory]");
            PowerupInventory = inventoryContainer.AddComponent<PowerupCarousel>();
            PowerupInventory.Initialize();

            p.PrimaryCursorChanged += PrimaryCursorChanged;

            // Make the powerup inventory be under the appropriate cursor.
            SetInventoryCursor(p.SecondaryCursor);

            if (cursor.Input == null)
            {
                cursor.ControllerFound += AddInputListeners;
            }
            else
            {
                AddInputListeners(cursor.Input);
            }
        }

        private void AddInputListeners(VRTK.VRTK_ControllerEvents input)
        {
            cursor.Input.TouchpadTouchStart += TouchpadTouchStart;
            cursor.Input.TouchpadTouchEnd += TouchpadTouchEnd;
        }

        private void TouchpadTouchStart(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            PowerupInventory.PresentPowerups();
        }

        private void TouchpadTouchEnd(object sender, VRTK.ControllerInteractionEventArgs e)
        {
            PowerupInventory.HidePowerups();
        }

        private void SetInventoryCursor(Cursor c)
        {
            cursor = c;
            PowerupInventory.transform.SetParent(cursor.transform);
            PowerupInventory.transform.localPosition = cursor.InventoryTransform.localPosition;
        }

        private void PrimaryCursorChanged(Cursor primary, Cursor secondary)
        {
            SetInventoryCursor(secondary);
        }
    }
}

