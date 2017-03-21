using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InternetGame {

	[CustomEditor(typeof(Lightbar))]
	public class LightbarEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();

			if (GUILayout.Button ("Add Packet")) {

				Lightbar lightbar = (Lightbar)target;
				lightbar.AddLight (lightbar.NextPacketColor, 1, 15.0f);
			}

			if (GUILayout.Button ("Change Time Left")) {

				Lightbar lightbar = (Lightbar)target;
				lightbar.ZoomLight (2.0f);
			}
		}
	}
}