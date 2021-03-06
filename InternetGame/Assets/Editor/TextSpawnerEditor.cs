using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InternetGame {

	[CustomEditor(typeof(TextSpawner))]
	public class TextSpawnerEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();

			if (GUILayout.Button ("Add Text")) {
				((TextSpawner)target).AddText ("15", Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1));
			}
		}
	}
}