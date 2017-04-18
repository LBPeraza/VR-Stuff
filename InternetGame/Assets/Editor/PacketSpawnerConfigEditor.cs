using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InternetGame
{
    [CustomEditor(typeof(LevelControllerConfig))]
    public class LevelControllerConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save spawn config (check config name!)"))
            {
                LevelControllerConfig config = (LevelControllerConfig) target;
                config.SaveToFile(config.Name);
            }

            if (GUILayout.Button("Load spawn config (check config name!)"))
            {
                LevelControllerConfig config = (LevelControllerConfig)target;
                config.LoadFromFile(config.Name);
            }
        }
    }
}