using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PowerupCarousel : PowerupInventory
    {
        protected class PowerupStack : MonoBehaviour
        {
            public List<Powerup> Powerups;
            public float TopDiscVerticalOffset;
            public float DiscGap;

            protected bool presenting;
            protected bool dirty;

            public void Initialize(float topDiscOffset, float discGap)
            {
                Powerups = new List<Powerup>();

                TopDiscVerticalOffset = topDiscOffset;
                DiscGap = discGap;

                presenting = false;
                dirty = false;
            }

            public void Add(Powerup p)
            {
                p.transform.SetParent(this.transform);

                Powerups.Add(p);
                dirty = true;

                if (presenting)
                {
                    Present();
                }
            }

            public void Remove(Powerup p)
            {
                // Make powerup a top-level object.
                p.transform.SetParent(null);

                Powerups.Remove(p);
                dirty = true;
            }

            public void Present()
            {
                if (Powerups.Count > 0)
                {
                    if (dirty)
                    {
                        // Only redraw if we have changed since before.
                        Vector3 powerupPosition = new Vector3(0, 0, 0);
                        int index = 0;
                        foreach (Powerup powerup in Powerups)
                        {
                            powerup.transform.localPosition = powerupPosition;

                            if (index == Powerups.Count - 1)
                            {
                                powerup.transform.localPosition += new Vector3(0, TopDiscVerticalOffset);
                            }

                            index++;
                            powerupPosition += new Vector3(0, DiscGap);
                        }
                    }

                    Powerup topOfStack = Powerups[Powerups.Count - 1];
                    topOfStack.PresentForUse();
                }

                dirty = false;
                presenting = true;
            }

            public void Hide()
            {
                if (Powerups.Count > 0)
                {
                    Powerup topOfStack = Powerups[Powerups.Count - 1];
                    topOfStack.Stow();
                }

                presenting = false;
            }

            public void SetAlpha(float alpha)
            {
                foreach (Powerup p in Powerups)
                {
                    p.SetAlpha(alpha);
                }
            }
        }

        [Header("Carousel Settings")]
        public float CarouselRadius = 0.2f; // meters
        public float DiscGap = 0.01f; // meters
        public float TopDiscVerticalOffset = 0.13f; // meters
        public float FadeRate = 2.0f;
        public GameObject CarouselContainer;

        protected Dictionary<PowerupType, PowerupStack> powerupStacks;

        private float previousGameTime;
        private float fadeProgress; // 0.0 - 1.0
        private Coroutine fadeAnimation;

        public override void Initialize()
        {
            powerupStacks = new Dictionary<PowerupType, PowerupStack>();

            if (CarouselContainer == null)
            {
                CarouselContainer = new GameObject("CarouselContainer");
                CarouselContainer.transform.SetParent(transform);
                CarouselContainer.transform.localPosition = Vector3.zero;
            }

            fadeProgress = 0.0f;

            base.Initialize();
        }

        public override bool Add(Powerup powerup)
        {
            bool succesful = base.Add(powerup);

            if (!powerupStacks.ContainsKey(powerup.Type))
            {
                // If we don't have a stack yet for this powerup type, add it in.
                GameObject container = new GameObject("PowerupStack");
                container.transform.SetParent(CarouselContainer.transform);
                PowerupStack stack = container.AddComponent<PowerupStack>();
                stack.Initialize(TopDiscVerticalOffset, DiscGap);

                powerupStacks.Add(powerup.Type, stack);
            }

            powerupStacks[powerup.Type].Add(powerup);

            return succesful;
        }

        public override bool Remove(Powerup p)
        {
            bool succesful = base.Remove(p);

            if (succesful)
            {
                PowerupStack powerupStack = powerupStacks[p.Type];
                powerupStack.Remove(p);
            }
            return succesful;
        }

        public override void PresentPowerups()
        {
            int numStacks = powerupStacks.Keys.Count;
            float angle = 0.0f;
            float angleIncrement = 360.0f / numStacks;
            Vector3 baseStackPosition = new Vector3(CarouselRadius, 0);

            // Make a stack for each kind of powerup.
            foreach (var powerupEntry in powerupStacks)
            {
                var powerupType = powerupEntry.Key;
                var powerupStack = powerupEntry.Value;

                Vector3 stackPosition = Quaternion.Euler(0, angle, 0) * baseStackPosition;
                powerupStack.transform.localPosition = stackPosition;

                powerupStack.Present();

                angle += angleIncrement;
            }

            BeginFade(true /* fade in */, () => { });
        }

        public override void HidePowerups()
        {
            BeginFade(false /* fade out */, () => {
                foreach (var powerupStack in powerupStacks.Values)
                {
                    powerupStack.Hide();
                }
            });
        }

        private void BeginFade(bool fadeIn, Action callback)
        {
            if (fadeAnimation != null)
            {
                StopCoroutine(fadeAnimation);
            }

            fadeAnimation = StartCoroutine(Fade(fadeIn, callback));
        }

        private void SetVisible(bool visible)
        {
            var renderers = CarouselContainer.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = visible;
            }
        }

        private IEnumerator Fade(bool fadeIn, Action callback)
        {
            if (fadeIn)
            {
                SetVisible(true);
            }

            previousGameTime = GameManager.GetInstance().GameTime();
            while ((fadeProgress < 1.0f && fadeIn) ||
                (fadeProgress > 0.0f && !fadeIn))
            {
                float deltaTime = GameManager.GetInstance().GameTime() - previousGameTime;
                fadeProgress = Mathf.Clamp01(fadeProgress + ((fadeIn ? 1 : -1) * FadeRate * deltaTime));

                foreach (PowerupStack stack in powerupStacks.Values)
                {
                    stack.SetAlpha(fadeProgress);
                }

                previousGameTime = GameManager.GetInstance().GameTime();

                yield return null;
            }

            if (!fadeIn)
            {
                SetVisible(false);
            }
            callback();
        }
    }
}

