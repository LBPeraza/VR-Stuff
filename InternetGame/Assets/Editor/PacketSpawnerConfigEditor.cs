using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InternetGame
{
    [CustomEditor(typeof(PacketSpawnerConfig))]
    public class PacketSpawnerConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save spawn config (check config name!)"))
            {
                PacketSpawnerConfig config = (PacketSpawnerConfig) target;
                config.SaveToFile(config.Name);
            }

            if (GUILayout.Button("Load spawn config (check config name!)"))
            {
                PacketSpawnerConfig config = (PacketSpawnerConfig)target;
                config.LoadFromFile(config.Name);
            }
        }
    }
}