using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InternetGame {

	[CustomEditor(typeof(Lightbar))]
	public class LightbarEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();

			if (GUILayout.Button ("AddPacket")) {

				Lightbar lightbar = (Lightbar)target;
				lightbar.AddLight (lightbar.NextPacketColor, 1, 15.0f);
			}
		}
	}
}