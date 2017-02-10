using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InternetGame {
	
	[CustomEditor(typeof(PortLoader))]
	public class PortLoaderEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();

			if (GUILayout.Button ("Save Level (check level name!)")) {

				PortLoader loader = (PortLoader)target;
				loader.SavePorts ();

			}
		}
	}
}