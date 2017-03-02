using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InternetGame
{
    [CustomEditor(typeof(LevelParameters))]
    public class LevelParameterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save level params (check params name!)"))
            {
                LevelParameters config = (LevelParameters)target;
                config.SaveToFile(config.LevelName);
            }

            if (GUILayout.Button("Load level params (check params name!)"))
            {
                LevelParameters config = (LevelParameters)target;
                config.LoadFromFile(config.LevelName);
            }
        }
    }
}