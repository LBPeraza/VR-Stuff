using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InternetGame {
	
	[CustomEditor(typeof(ScoreSaver))]
	public class ScoreSaverEditor : Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector ();

			if (GUILayout.Button ("Save test score")) {
				ScoreSaver saver = (ScoreSaver)target;
				saver.TestSaveScore ();
			}
		}
	}
}

