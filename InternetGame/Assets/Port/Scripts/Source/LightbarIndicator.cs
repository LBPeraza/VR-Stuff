using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	struct LightChunk {
		public Color color;
		public int size;
		public float startTime;
		public float duration;

		public LightChunk (Color color, int size, float startTime, float duration) {
			this.color = color;
			this.size = size;
			this.startTime = startTime;
			this.duration = duration;
		}
	}

	[ExecuteInEditMode]
	public class LightbarIndicator : MonoBehaviour {

		public int NumLights = 20;

		public float SegmentSeparation = 0.033f;

		public GameObject LightbarSegmentPrefab;

		[Header("Add Packets")]
		public Color NextPacketColor;

		private List<GameObject> LightbarSegments;
		private List<LightChunk> Lights;

		private bool Dirty = false;

		// Use this for initialization
		void Start () {
			if (LightbarSegmentPrefab == null) {
				LightbarSegmentPrefab = Resources.Load <GameObject>("Prefabs/LightbarSegment");
			}
			LightbarSegments = new List<GameObject> ();
			ClearChildren ();
			CreateSegments ();
			Lights = new List<LightChunk> ();
		}

		void OnValidate() {
			NumLights = Mathf.Max (0, NumLights);
			Dirty = true;
		}

		void ClearChildren() {
			List<GameObject> toDelete = new List<GameObject>();

			foreach (Transform child in transform) {
				toDelete.Add(child.gameObject);
			}

			foreach (GameObject obj in toDelete)
			{
				DestroyImmediate (obj);
			}
			LightbarSegments.Clear ();
		}

		void CreateSegments() {
			for (int i = 0; i < NumLights; i++) {
				GameObject light = Instantiate (LightbarSegmentPrefab, transform);
				Vector3 pos = light.transform.localPosition;
				pos.x = 0.0f; pos.y = -i * SegmentSeparation; pos.z = 0.0f;
				light.transform.localPosition = pos;
				float scale = 100.0f - i * 100.0f / (NumLights - 1);
				light.GetComponentInChildren<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, scale);
				LightbarSegments.Add (light);
			}
		}

		// Update is called once per frame
		void Update () {
			if (Application.isEditor && Dirty && (Lights == null || Lights.Count != NumLights)) {
				Lights = new List<LightChunk> ();
				ClearChildren ();
				CreateSegments ();
				Dirty = false;
			}

			if (Application.isPlaying) {
				float currentTime = Time.fixedTime;
				List<LightChunk> newChunks = new List<LightChunk> ();
				foreach (GameObject seg in LightbarSegments) {
					seg.GetComponentInChildren<SkinnedMeshRenderer> ().materials [1].color = Color.black;
				}
				foreach (LightChunk chunk in Lights) {
					float timeLeft = chunk.duration - (currentTime - chunk.startTime);
					if (timeLeft > 0.0f) {
						newChunks.Add (chunk);
						int i = Mathf.CeilToInt (Mathf.Lerp (-1, NumLights - 1, timeLeft / chunk.duration));
						LightbarSegments [i].GetComponentInChildren<SkinnedMeshRenderer> ().materials [1].color = chunk.color;
					}
				}
				Lights = newChunks;
			}
		}

		public void AddLight(Color lightColor, int lightLength, float duration) {
			float currentTime = Time.fixedTime;
			Lights.Add (new LightChunk (
				lightColor, lightLength, currentTime, duration));
		}
	}
}