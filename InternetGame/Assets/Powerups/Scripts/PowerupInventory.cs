using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {

	using PowerupDict = Dictionary<PowerupType, List<Powerup>>;

	public class PowerupInventory : MonoBehaviour {
		
		private PowerupDict Powerups;

		public void Initialize() {
			Powerups = new PowerupDict ();
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
				return true;
			}
			return false;
		}

		public Powerup GetPowerup(PowerupType powerupType) {
			List<Powerup> powerups = GetPowerups (powerupType);
			int count = powerups.Count;
			if (count > 0) {
				Powerup powerup = powerups [count - 1];
				powerups.RemoveAt (count - 1);
				return powerup;
			}
			return null;
		}

	}
}