using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TowerBuilder : MonoBehaviour {
	public int Height;
	public GameObject Brick;

	void Start() {
		if (!Application.isPlaying) {
			Debug.Log ("Rebuilding tower");

			Transform bricks = this.transform.FindChild ("bricks");	
			if (bricks == null) {
				bricks = new GameObject().transform;
				bricks.name = "bricks";
				bricks.SetParent (this.transform);
			}

			if (bricks.childCount != Height) {
				foreach (Transform child in bricks) {
					DestroyImmediate (child.gameObject);
				}

				for (int i = 0; i < Height; i++) {
					GameObject brick = (GameObject) Instantiate(Brick, new Vector3(0, i, 0), Quaternion.identity);
					brick.transform.SetParent (bricks);
				}
			}
		}
	}
}
