using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace InternetGame {
	
	public enum PowerupType {
		Unset,
		Invincibility,
		TimeFreeze
	}

	enum PowerupState {
        Unset,
		AwaitingCollection,
		Stored,
        Presenting,
		Held
	}

	public abstract class Powerup : MonoBehaviour
    {
		private PowerupState state;
		private GameManager manager;
		private float creationTime;
		private Vector3 origin;
		private List<float> originalAlphas;

		protected float mDuration;
        protected bool initialized;
        protected PowerupInteractableObject interactable;
        protected PowerupInventory inventory;

		public abstract PowerupType Type { get; }
		public abstract float Duration { get; }
		public abstract int MaxCount { get; }
		public abstract Color PowerupColor { get; }

        public virtual float PickUpFloatAmplitude { get { return 0.1f; } }
        public virtual float PickUpFloatPeriod { get { return 6.0f; } }

        public virtual float PresentingFloatAmplitude { get { return 0.01f;  } }
        public virtual float PresentingFloatPeriod { get { return 8.0f;  } }

		public virtual float RotatePeriod { get { return 6.5f; } }

		public virtual float FlashDuration { get { return 5.0f; } }
		public virtual float TimeToPickUp { get { return 10.0f; } }

		public virtual void Initialize() {
			manager = GameManager.GetInstance ();
			creationTime = manager.GameTime ();

            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            originalAlphas = new List<float> ();
			if (renderer != null) {
				renderer.materials [2].color = PowerupColor;
				foreach (Material mat in renderer.materials) {
					originalAlphas.Add (mat.color.a);
				}
			} else {
				Debug.Log ("No mesh renderer found.");
			}

            interactable = GetComponent<PowerupInteractableObject>();
            interactable.Initialize(this);

            origin = transform.localPosition;
            state = PowerupState.Unset;
            initialized = true;
        }

        public virtual void OnCollected(PowerupInventory inventory)
        {

        }

        public virtual void Pickup(PowerupInventory inventory)
        {
            this.inventory = inventory;
            inventory.Add(this);

            OnCollected(inventory);

            Stow();
        }

        public virtual void PresentForCollection()
        {
            origin = transform.localPosition;
            state = PowerupState.AwaitingCollection;

            // Can only touch the powerup, not grab it.
            SetGrabbable(false);
        }

        public virtual void PresentForUse()
        {
            origin = transform.localPosition;
            transform.localRotation = Quaternion.identity;
            state = PowerupState.Presenting;

            SetGrabbable(true);
        }

        public virtual void Stow()
        {
            transform.localPosition = origin;

            // Rotate onto its side.
            transform.localRotation = Quaternion.Euler(90, 0, 0);
            state = PowerupState.Stored;

            SetGrabbable(false);
        }

        public virtual void OnHold()
        {
            state = PowerupState.Held;

            inventory.Remove(this);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            switch (state)
            {
                case PowerupState.AwaitingCollection:
                    if (collision.collider.CompareTag("Player"))
                    {
                        Debug.Log("collided with player");
                        // This will update state to "Stored."
                        Pickup(GameManager.GetInstance().Player.InventoryController.PowerupInventory);
                    }
                    break;
                default:
                    break;
            }
        }

        public virtual void OnGrabbed(InteractableObjectEventArgs e)
        {
            switch (state)
            {
                case PowerupState.Presenting:
                    VRTK_InteractGrab grab = e.interactingObject.GetComponentInParent<VRTK_InteractGrab>();
                    Cursor cursor = grab.controllerAttachPoint.GetComponentInParent<Cursor>();

                    cursor.OnGrab(Cursor.DefaultCursorEventArgs);

                    OnHold();
                    break;
                default:
                    break;
            }
        }

        public virtual void OnDropped(bool inSnapZone)
        {
            if (state == PowerupState.Held)
            {
                if (inSnapZone)
                {
                    // After you've inserted the powerup, you can't undo it.
                    SetGrabbable(false);

                    // Override VRTK behavior that reverts to previous parent.
                    transform.SetParent(null);
                }
                else
                {
                    inventory.Add(this);
                    //PresentForCollection();
                }

                LinkController.GetInstance().Cursor.OnDrop(Cursor.DefaultCursorEventArgs);
            }
        }

        public virtual void SetGrabbable(bool grabbable)
        {
            interactable.SetGrabbable(grabbable);
        }

        public void SetAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            Material[] mats = GetComponentInChildren<MeshRenderer>().materials;
            for (int i = 0; i < mats.Length; i++)
            {
                Color col = mats[i].color;
                col.a = alpha * originalAlphas[i];
                mats[i].color = col;
            }
        }

        private void flash(float liveTime)
        {
            float alpha = 0.0f;
            if (liveTime >= TimeToPickUp)
            {
                alpha = 1.0f;
            }
            else
            {
                float flashTime = liveTime - (TimeToPickUp - FlashDuration);

                //float f = 1 - 0.2f * Mathf.Pow (0.1f, FlashDuration - flashTime);
                float f = 1.0f - 0.3f * flashTime / FlashDuration;

                alpha = 0.5f * (1 + Mathf.Cos(2 * Mathf.PI * flashTime / f));
                //alpha = 1.0f - Mathf.Pow (Mathf.Cos (2 * Mathf.PI * flashTime / f), 2.0f);
            }

            SetAlpha(alpha);
        }

        private void rotate(float time)
        {
            transform.localRotation = Quaternion.identity;
            transform.RotateAround(transform.position, transform.up, time * 360 / RotatePeriod);
        }

        private void bob(float time, float amplitude, float period)
        {
            float posOffset = amplitude * Mathf.Sin(2 * Mathf.PI * time / period);
            transform.localPosition = origin + Vector3.up * posOffset;
        }

        public virtual void Update()
        {
            if (!initialized)
            {
                // Don't run update unless initialized.
                return;
            }

            float liveTime = manager.GameTime() - creationTime;

            switch (state)
            {
                case PowerupState.AwaitingCollection:
                    bob(liveTime, PickUpFloatAmplitude, PickUpFloatPeriod);
                    rotate(liveTime);

                    if (liveTime < TimeToPickUp)
                    {
                        SetAlpha(liveTime / TimeToPickUp);
                    }
                    else
                    {
                        DestroyImmediate(gameObject);
                    }
                    break;
                case PowerupState.Presenting:
                    bob(liveTime, PresentingFloatAmplitude, PresentingFloatPeriod);
                    rotate(liveTime);
                    break;
                default:
                    break;
            }
        }
    }
}