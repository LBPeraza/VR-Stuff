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

        public int NumDroppedPacketsAllowed;
        public Soundtrack BackgroundSoundtrack;
        public PacketSpawnerType PacketSpawner;

        [NonSerialized]
        public PacketSpawnerConfig PacketSpawnerConfig;

        public static string path = "Assets/Levels/LevelData/";

        public void LoadFromFile(string fileName)
        {
            using (FileStream fs = new FileStream(path + fileName, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string json = reader.ReadToEnd();
                    JsonUtility.FromJsonOverwrite(json, this);
                }
            }

            switch (PacketSpawner)
            {
                case PacketSpawnerType.Wave:
                    // Load wave config from file.
                    PacketSpawnerConfig = new PacketSpawnerConfig();
                    PacketSpawnerConfig.LoadFromFile(LevelName);
                    break;
            }
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

