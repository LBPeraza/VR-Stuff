using UnityEngine;
using System.Collections;

public class Slingshot : Holdable {
	private GameObject slingHand;
	private bool aiming = false;

	public float aimThreshold = 1.0f;

	public override void PickUp (GameObject yokeHand, GameObject slingHand) {
		base.PickUp (yokeHand, slingHand);
		this.slingHand = slingHand;
	}

	void StartShot () {
		Transform slingTransform = slingHand.transform;
		if (!aiming && Vector3.Distance (slingTransform.position, transform.position) < aimThreshold) {
			aiming = true;
		}
	}
}
