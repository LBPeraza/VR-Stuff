using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PowerupCarousel : MonoBehaviour
    {
        [Header("Carousel Settings")]
        public float CarouselRadius = 0.2f; // meters
        public float DiscGap = 0.01f; // meters
        public float TopDiscVerticalOffset = 0.13f; // meters
        public GameObject CarouselContainer;

        protected Dictionary<PowerupType, GameObject> powerupStacks;
        protected PowerupInventory inventory;

        public void Initialize(PowerupInventory powerupInventory)
        {
            powerupStacks = new Dictionary<PowerupType, GameObject>();

            if (CarouselContainer == null)
            {
                CarouselContainer = new GameObject("CarouselContainer");
                CarouselContainer.transform.SetParent(transform);
                CarouselContainer.transform.localPosition = Vector3.zero;
            }

            inventory = powerupInventory;
            inventory.PowerupAdded += OnPowerupAdded;
        }

        private void OnPowerupAdded(object sender, PowerupEventArgs e)
        {
            
        }

        private void Present()
        {
            int numStacks = inventory.Powerups.Keys.Count;
            float angle = 0.0f;
            float angleIncrement = 360.0f / numStacks;
            Vector3 baseStackPosition = new Vector3(CarouselRadius, 0);

            // Make a stack for each kind of powerup.
            foreach (var powerupEntry in inventory.Powerups)
            {
                var powerupType = powerupEntry.Key;
                var powerupList = powerupEntry.Value;

                if (!powerupStacks.ContainsKey(powerupType)) {
                    // If we don't have a stack yet for this powerup type, add it in.
                    GameObject container = new GameObject("PowerupStack");
                    container.transform.SetParent(CarouselContainer.transform);
                    powerupStacks.Add(powerupType, container);
                }

                Transform powerupStack = powerupStacks[powerupType].transform;

                Vector3 stackPosition = Quaternion.Euler(0, angle, 0) * baseStackPosition;
                powerupStack.localPosition = stackPosition;

                PresentStack(powerupStack, powerupList);

                angle += angleIncrement;
            }
        }

        private void PresentStack(Transform stackContainer, List<Powerup> stack)
        {
            Vector3 powerupPosition = new Vector3(0, 0, 0);
            int index = 0;
            foreach (Powerup powerup in stack)
            {
                powerup.transform.SetParent(stackContainer);
                powerup.transform.localPosition = powerupPosition;

                if (index == stack.Count - 1)
                {
                    powerup.transform.localPosition += new Vector3(0, TopDiscVerticalOffset);
                    powerup.Present();
                }

                index++;
                powerupPosition += new Vector3(0, DiscGap);
            }
        }

        public void Stow()
        {

        }

        public void Unstow()
        {
            Present();
        }
    }
}

