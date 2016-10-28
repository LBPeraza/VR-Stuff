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
        shaft.localPosition = Vector3.zero;

        Transform shaftBricks = shaft.FindChild("bricks");
        if (shaftBricks == null)
        {
            shaftBricks = new GameObject().transform;
            shaftBricks.name = "bricks";
            shaftBricks.SetParent(shaft);
        }
        shaftBricks.localPosition = Vector3.zero;


        if (shaftBricks.childCount != Height) {
			foreach (Transform child in shaftBricks) {
				DestroyImmediate (child.gameObject);
			}

			for (int i = 0; i < Height; i++) {
				GameObject brick = (GameObject) Instantiate(Brick, Vector3.zero, Quaternion.identity);
				brick.transform.SetParent (shaftBricks);
                brick.transform.localPosition = new Vector3(0, i + (brick.transform.localScale.y / 2), 0);
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
        platform.localPosition = new Vector3(0, Height, 0);

        Transform platformBricks = platform.FindChild("bricks");
        if (platformBricks == null)
        {
            platformBricks = new GameObject().transform;
            platformBricks.name = "bricks";
            platformBricks.SetParent(platform);
        }
        platformBricks.localPosition = Vector3.zero;


        if (platformBricks.childCount != PlatformWidth * PlatformWidth) {
			foreach (Transform child in platformBricks) {
				DestroyImmediate (child.gameObject);
			}

			for (int i = 0; i < PlatformWidth * PlatformWidth; i++) {
				GameObject brick = (GameObject) Instantiate(Brick, Vector3.zero, Quaternion.identity);
				brick.transform.SetParent (platformBricks);
                brick.transform.localPosition = new Vector3((i % PlatformWidth) - (PlatformWidth / 2),
                    0, (i / PlatformWidth) - (PlatformWidth / 2));
                brick.transform.localScale = new Vector3 (1, 0.25f, 1);
			}
		}
	}
}
