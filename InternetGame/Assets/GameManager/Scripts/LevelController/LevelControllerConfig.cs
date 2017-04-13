using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace InternetGame
{
    [Serializable]
    public class LevelControllerConfig : MonoBehaviour
    {
        public string Name;
        public WaveConfig[] Waves;
        public static string path = "Assets/Levels/LevelData/SpawnConfigs/";

        public bool LoadFromFile(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(path + fileName, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        string json = reader.ReadToEnd();
                        JsonUtility.FromJsonOverwrite(json, this);
                    }
                }

                return true;
            }
            catch (Exception  e)
            {
                return false;
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
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }

    [Serializable]
    public class WaveConfig
    {
        public float MarginBefore;
        public float MarginAfter;

        public PacketConfig[] Packets;
    }

    [Serializable]
    public class PacketConfig
    {
        public string Destination;
        public PacketPayloadType Type;
        public int Size;

        public string Source;

        public float Offset;
    }
}
