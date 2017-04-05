using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	
	public enum PowerupType {
		Invincibility,
		TimeFreeze
	}

	public abstract class Powerup : MonoBehaviour {

		protected float mDuration;

		public abstract PowerupType Type { get; }
		public abstract float Duration { get; }
		public abstract int MaxCount { get; }

		public static Powerup MakePowerup(PowerupType powerupType) {
			switch (powerupType) {
			case PowerupType.Invincibility:
				return new InvincibilityPowerup ();
			case PowerupType.TimeFreeze:
				return new TimeFreezePowerup ();
			default:
				Debug.Log ("This is theoretically impossible.");
				return null;
			}
		}

		public virtual void Initialize() {
		}
	}
}