using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSpawner : MonoBehaviour {

	public static GameObject TextCanvasPrefab;
	public GameObject TextCanvas;

	void Start() {
		Initialize ();
	}

	public void Initialize() {
		if (TextCanvasPrefab == null)
			TextCanvasPrefab = Resources.Load<GameObject> ("Prefabs/PopupTextCanvas");

		if (TextCanvas == null) {
			TextCanvas = Instantiate (TextCanvasPrefab, transform);
			TextCanvas.transform.localPosition = Vector3.zero;
		}
	}

	void Update() {
		if (TextCanvas) {
			Vector3 v = Camera.main.transform.position - TextCanvas.transform.position;
			v.x = v.z = 0.0f;
			TextCanvas.transform.LookAt (Camera.main.transform.position - v);
			TextCanvas.transform.Rotate (0, 180, 0);
		}
	}
}
