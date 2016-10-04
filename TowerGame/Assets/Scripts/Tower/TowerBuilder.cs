using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TowerBuilder : MonoBehaviour {
	public uint Height;
	public GameObject Brick;
	public uint PlatformWidth;

	void Start() {
		if (!Application.isPlaying) {
			Debug.Log ("Rebuilding tower");
			MakeTowerShaft ();
			MakeTowerPlatform ();
		}
	}

	void MakeTowerShaft() {
		Transform shaft = this.transform.FindChild ("shaft");	
		if (shaft == null) {
			shaft = new GameObject().transform;
			shaft.name = "shaft";
			shaft.SetParent (this.transform);
		}

		if (shaft.childCount != Height) {
			foreach (Transform child in shaft) {
				DestroyImmediate (child.gameObject);
			}

			for (int i = 0; i < Height; i++) {
				GameObject brick = (GameObject) Instantiate(Brick, new Vector3(0, i, 0), Quaternion.identity);
				brick.transform.SetParent (shaft);
			}
		}
	}

	void MakeTowerPlatform() {
		Transform platform = this.transform.FindChild ("platform");
		if (platform == null) {
			platform = new GameObject ().transform;
			platform.name = "platform";
			platform.SetParent (this.transform);
		}

		if (platform.childCount != PlatformWidth * PlatformWidth) {
			foreach (Transform child in platform) {
				DestroyImmediate (child.gameObject);
			}

			for (int i = 0; i < PlatformWidth * PlatformWidth; i++) {
				GameObject brick = (GameObject) Instantiate(Brick, new Vector3((i % PlatformWidth) - (PlatformWidth / 2),
					Height, (i / PlatformWidth) - (PlatformWidth / 2)), Quaternion.identity);
				brick.transform.SetParent (platform);
				brick.transform.localScale = new Vector3 (1, 0.25f, 1);
			}
		}
	}
}
