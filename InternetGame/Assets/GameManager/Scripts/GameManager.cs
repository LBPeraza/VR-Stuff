using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public struct GameScore
    {
        public int BytesDelivered;
        public int PacketsDelivered;
        public int PacketsDropped;
        public float VirusAmount;
        public float NumberOfVirusesInfected;
        public float NumberOfVirusesStopped;
    }

    public class GameManager : MonoBehaviour
    {
        public GameObject PacketSources;
        public GameObject PacketSinks;

        public static List<PacketSource> AllPacketSources;
        public static List<PacketSink> AllPacketSinks;

        public PacketSpawner PacketSpawner;
        public Player Player;
        public InputManager InputManager;

        public static GameScore Score;

        public static string PacketSinksPath = "/Sinks";
        public static string PacketSourcesPath = "/Sources";

        // Use this for initialization
        void Start()
        {
            Initialize();

            InputManager.Initialize();
            Player.Initialize();

            if (PacketSpawner != null)
            {
                PacketSpawner.Initialize();
            }

            ResetScore(Score);
        }

        public void Initialize()
        {
            if (PacketSinks == null)
            {
                PacketSinks = GameObject.Find(PacketSinksPath);
            }

            if (PacketSources == null)
            {
                PacketSources = GameObject.Find(PacketSourcesPath);
            }

            AllPacketSources = new List<PacketSource>(PacketSources.GetComponentsInChildren<PacketSource>());
            AllPacketSinks = new List<PacketSink>(PacketSinks.GetComponentsInChildren<PacketSink>());
        }

        public static void AddVirus(Virus v)
        {
            Score.VirusAmount += v.Damage;
            Score.NumberOfVirusesInfected++;
        }

        public static void ReportStoppedVirus(Virus v)
        {
            Score.NumberOfVirusesStopped++;
        }

        public void ResetScore(GameScore score)
        {
            score.BytesDelivered = 0;
            score.PacketsDelivered = 0;
            score.PacketsDropped = 0;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
