using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InternetGame
{
    public enum PacketSpawnerType
    {
        Infinite,
        MainMenu,
        Wave
    }

    [Serializable]
    public class LevelParameters : MonoBehaviour
    {
        public string LevelName;
        public string PortMapName;
        public int NumDroppedPacketsAllowed;
        public Soundtrack BackgroundSoundtrack;
        public PacketSpawnerType PacketSpawner;

        [NonSerialized]
        public PacketSpawnerConfig PacketSpawnerConfig;

        public static string path = "Assets/Levels/LevelData/";

        public static LevelParameters LoadFromFile(string fileName, LevelParameters levelParams = null)
        {
            if (levelParams == null)
            {
                GameObject levelParamContainer = new GameObject("LevelParametersTemp");
                levelParams = levelParamContainer.AddComponent<LevelParameters>();
            }
            using (FileStream fs = new FileStream(path + fileName, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string json = reader.ReadToEnd();
                    JsonUtility.FromJsonOverwrite(json, levelParams);
                }
            }

            switch (levelParams.PacketSpawner)
            {
                case PacketSpawnerType.Wave:
                    // Load wave config from file.
                    levelParams.PacketSpawnerConfig = new PacketSpawnerConfig();
                    levelParams.PacketSpawnerConfig.LoadFromFile(fileName);
                    break;
            }
            return levelParams;
        }

        public void SaveToFile(string fileName)
        {
            using (FileStream fs = new FileStream(path + fileName, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    string json = JsonUtility.ToJson(this);
                    writer.Write(json);
                    Debug.Log("Writing to file!");
                    writer.Flush();
                    writer.Close();
                }
            }

            if (PacketSpawnerConfig != null)
            {
                PacketSpawnerConfig.SaveToFile(LevelName);
            }
        }
    }
}

