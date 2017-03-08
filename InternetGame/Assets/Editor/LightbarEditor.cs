using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InternetGame {

	[CustomEditor(typeof(LightbarIndicator))]
	public class LightbarEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();

			if (GUILayout.Button ("AddPacket")) {

				LightbarIndicator lightbar = (LightbarIndicator)target;
				lightbar.AddLight (lightbar.NextPacketColor, 1, 15.0f);

			}
		}
	}
}