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
		public abstract Color PowerupColor { get; }

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
			MeshRenderer renderer = GetComponentInChildren<MeshRenderer> ();
			if (renderer != null) {
				renderer.materials [2].color = PowerupColor;
				Debug.Log (PowerupColor);
			} else {
				Debug.Log ("No mesh renderer found.");
			}
		}

		void Start() {
			Initialize ();
			Debug.Log (transform.parent.name);
		}
	}
}