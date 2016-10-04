using UnityEngine;
using System.Collections;

public class Holdable : MonoBehaviour {
	private GameObject holder;
	private bool isHeld;

	public float grabThreshold = 1.0f;

	void Start () {
		holder = null;
		isHeld = false;
		this.transform.tag = "Holdable";
	}

	public virtual void PickUp (GameObject hand, GameObject otherHand) {
		Transform handTransform = hand.transform;
		if (!isHeld && Vector3.Distance(handTransform.position, transform.position) < grabThreshold) {
			holder = hand;
			isHeld = true;
            this.GetComponent<Rigidbody>().isKinematic = true;
			this.transform.position = handTransform.position;
			this.transform.rotation = handTransform.rotation;
			this.transform.SetParent (holder.transform);
		}
	}

	public void PutDown () {
		this.transform.SetParent (null);
		holder = null;
		isHeld = false;
        this.GetComponent<Rigidbody>().isKinematic = false;
    }

    public bool IsHeld () {
		return isHeld;
	}
}
