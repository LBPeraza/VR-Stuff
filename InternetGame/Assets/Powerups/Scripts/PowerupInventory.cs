using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
    using System;
    using PowerupDict = Dictionary<PowerupType, List<Powerup>>;

    public enum PowerUpInventoryState
    {
        Unset,
        Stowed,
        Unstowing,
        Unstowed
    }

    public class PowerupEventArgs : EventArgs
    {
        public Powerup Powerup;
    }

	public class PowerupInventory : MonoBehaviour {
        public PowerupCarousel Carousel;

        public event EventHandler<PowerupEventArgs> PowerupAdded;
        public event EventHandler<PowerupEventArgs> PowerupRemoved;

        public PowerupDict Powerups;
        protected PowerUpInventoryState state;

		public void Initialize() {
			Powerups = new PowerupDict ();
            state = PowerUpInventoryState.Stowed;

            GameObject carouselContainer = new GameObject("PowerupCarousel");
            carouselContainer.transform.SetParent(this.transform);
            carouselContainer.transform.localPosition = Vector3.zero;
            Carousel = carouselContainer.AddComponent<PowerupCarousel>();
            Carousel.Initialize(this);
		}

        public void Start()
        {
            Initialize();
            PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);
            PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);
            PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);

            PowerupFactory.CreatePowerup(PowerupType.TimeFreeze, Vector3.zero).Pickup(this);
            PowerupFactory.CreatePowerup(PowerupType.TimeFreeze, Vector3.zero).Pickup(this);
            PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);

            Carousel.Unstow();
        }

        private List<Powerup> GetPowerups(PowerupType powerupType) {
			List<Powerup> powerups;
			Powerups.TryGetValue (powerupType, out powerups);
			if (powerups == null) {
				powerups = new List<Powerup> ();
				Powerups.Add (powerupType, powerups);
			}
			return powerups;
		}

		public int Count(PowerupType powerupType) {
			List<Powerup> powerups = GetPowerups (powerupType);
			return powerups.Count;
		}

		public bool Add(Powerup powerup) {
			List<Powerup> powerups = GetPowerups(powerup.Type);
			if (powerups.Count < powerup.MaxCount) {
				powerups.Add (powerup);

                if (PowerupAdded != null)
                {
                    PowerupAdded.Invoke(this, new PowerupEventArgs { Powerup = powerup });
                }

				return true;
			}
			return false;
		}

		public Powerup GetPowerup(PowerupType powerupType) {
			List<Powerup> powerups = GetPowerups (powerupType);
			int count = powerups.Count;
			if (count > 0) {
				Powerup powerup = powerups [0];
				powerups.RemoveAt (0);

                if (PowerupRemoved != null)
                {
                    PowerupRemoved.Invoke(this, new PowerupEventArgs { Powerup = powerup });
                }

                return powerup;
			}
			return null;
		}
	}
}