using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternetGame {
	public class TextSpawner : MonoBehaviour {

		public struct TextCanvasObject {
			public GameObject TextCanvas;
			public Vector3 Velocity;
		}

		public static GameObject TextCanvasPrefab;
		private List<GameObject> TextCanvases;

		[Header("Motion parameters")]
		public Transform InitialPosition;
        public float MaxPositionNoise = 0.01f;

		public Vector3 InitialDirection;
		public float MinVelocity = 0.5f;
		public float MaxVelocity = 1.2f;

		public void Initialize() {
			if (TextCanvasPrefab == null)
				TextCanvasPrefab = Resources.Load<GameObject> ("Prefabs/PopupTextCanvas");

			if (InitialPosition == null)
				InitialPosition = transform;

			InitialDirection = Vector3.up + transform.forward;

			InitialDirection.Normalize ();

			TextCanvases = new List<GameObject> ();

		}

		public void AddText(string text, Color color) {
			GameObject TextCanvas = Instantiate (TextCanvasPrefab, InitialPosition);

			Text textObject = TextCanvas.GetComponentInChildren<Text> ();
			textObject.text = text;
			textObject.color = color;

			Vector3 textPosition = Random.onUnitSphere * MaxPositionNoise;
			TextCanvas.transform.localPosition = textPosition;

			Vector3 direction;
			do {
				direction = Random.onUnitSphere;
			} while (Vector3.Dot (direction, InitialDirection) < 0.8);

			direction *= Mathf.Lerp(MinVelocity, MaxVelocity, Random.value);

			Rigidbody rb = TextCanvas.GetComponent<Rigidbody> ();
			rb.velocity = direction;

			TextCanvases.Add (TextCanvas);

			Animator animator = TextCanvas.GetComponentInChildren<Animator> ();
			AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo (0)[0];
			Destroy (TextCanvas, clipInfo.clip.length);
		}

		void Update() {
			if (TextCanvases == null)
				Initialize ();

			List<GameObject> keep = new List<GameObject> ();
			foreach (GameObject TextCanvas in TextCanvases) {
				if (TextCanvas != null) {
					keep.Add (TextCanvas);

					Vector3 v = Camera.main.transform.position - TextCanvas.transform.position;
					v.x = v.z = 0.0f;

					TextCanvas.transform.LookAt (Camera.main.transform.position - v);
					TextCanvas.transform.Rotate (0, 180, 0);
				}
			}
			TextCanvases = keep;
		}
	}
}