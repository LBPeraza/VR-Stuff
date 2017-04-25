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

	public abstract class PowerupInventory : MonoBehaviour {
        public event EventHandler<PowerupEventArgs> PowerupAdded;
        public event EventHandler<PowerupEventArgs> PowerupRemoved;

        public PowerupDict Powerups;
        protected PowerUpInventoryState state;

		public virtual void Initialize() {
			Powerups = new PowerupDict ();
            state = PowerUpInventoryState.Stowed;

            Seed();

            HidePowerups();
		}

        public void Seed()
        {
            PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);
            //PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);
            //PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);

            //PowerupFactory.CreatePowerup(PowerupType.TimeFreeze, Vector3.zero).Pickup(this);
            //PowerupFactory.CreatePowerup(PowerupType.TimeFreeze, Vector3.zero).Pickup(this);
            //PowerupFactory.CreatePowerup(PowerupType.Invincibility, Vector3.zero).Pickup(this);
        }

        public abstract void PresentPowerups();

        public abstract void HidePowerups();

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

		public virtual bool Add(Powerup powerup) {
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

        public virtual bool Remove(Powerup p)
        {
            PowerupType powerupType = p.Type;

            List<Powerup> powerupList = Powerups[powerupType];
            if (powerupList != null)
            {
                return powerupList.Remove(p);
            }

            return false;
        }

		public virtual Powerup GetPowerup(PowerupType powerupType) {
			List<Powerup> powerups = GetPowerups (powerupType);
			int count = powerups.Count;
			if (count > 0) {
				Powerup powerup = powerups [0];
                Remove(powerup);

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