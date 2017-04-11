using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	
	public enum PowerupType {
		Invincibility,
		TimeFreeze
	}

	enum PowerupState {
		NotPickedUp,
		Stored,
		Held
	}

	public abstract class Powerup : MonoBehaviour {

		private PowerupState state;
		private GameManager manager;
		private float creationTime;
		private Vector3 origin;
		private List<float> originalAlphas;

		protected float mDuration;

		public abstract PowerupType Type { get; }
		public abstract float Duration { get; }
		public abstract int MaxCount { get; }
		public abstract Color PowerupColor { get; }

		public virtual float FloatAmplitude { get { return 0.1f; } }
		public virtual float FloatPeriod { get { return 6.0f; } }

		public virtual float RotatePeriod { get { return 6.5f; } }

		public virtual float FlashDuration { get { return 5.0f; } }
		public virtual float TimeToPickUp { get { return 10.0f; } }

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
			manager = GameManager.GetInstance ();
			creationTime = manager.GameTime ();
			origin = transform.position;
			state = PowerupState.NotPickedUp;
			MeshRenderer renderer = GetComponentInChildren<MeshRenderer> ();
			originalAlphas = new List<float> ();
			if (renderer != null) {
				renderer.materials [2].color = PowerupColor;
				foreach (Material mat in renderer.materials) {
					originalAlphas.Add (mat.color.a);
				}
			} else {
				Debug.Log ("No mesh renderer found.");
			}
		}

		void Start() {
			Initialize ();
		}

		private void SetAlpha(float liveTime) {
			float alpha;
			if (liveTime < TimeToPickUp - FlashDuration) {
				alpha = 1.0f;
			} else if (liveTime > TimeToPickUp) {
				alpha = 0.0f;
			} else {
				float flashTime = liveTime - (TimeToPickUp - FlashDuration);

				//float f = 1 - 0.2f * Mathf.Pow (0.1f, FlashDuration - flashTime);
				float f = 1.0f - 0.3f * flashTime / FlashDuration;

				alpha = 0.5f * (1 + Mathf.Cos (2 * Mathf.PI * flashTime / f));
				//alpha = 1.0f - Mathf.Pow (Mathf.Cos (2 * Mathf.PI * flashTime / f), 2.0f);
			}

			Material[] mats = GetComponentInChildren<MeshRenderer> ().materials;
			for (int i = 0; i < mats.Length; i++) {
				Color col = mats[i].color;
				col.a = alpha * originalAlphas[i];
				mats[i].color = col;
			}
		}

		void Update() {
			switch (state) {
			case PowerupState.NotPickedUp:
				float liveTime = manager.GameTime () - creationTime;
				float posOffset = FloatAmplitude * Mathf.Sin (2 * Mathf.PI * liveTime / FloatPeriod);
				transform.position = origin + Vector3.up * posOffset;
				transform.rotation = Quaternion.identity;
				transform.RotateAround (transform.position, Vector3.up, liveTime * 360 / RotatePeriod);

				if (liveTime < TimeToPickUp) {
					SetAlpha (liveTime);
				} else {
					DestroyImmediate (gameObject);
				}
				break;
			default:
				break;
			}
		}
	}
}