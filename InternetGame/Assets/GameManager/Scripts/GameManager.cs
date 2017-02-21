using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public float Time;
    }

    public struct LevelParameters
    {
        public int NumDroppedPacketsAllowed;
        public Soundtrack BackgroundSoundtrack;
		public string LevelName;
    }

    public class GameManager : MonoBehaviour
    {
        public GameObject PacketSources;
        public GameObject PacketSinks;

        public static List<PacketSource> AllPacketSources;
        public static List<PacketSink> AllPacketSinks;

        public SceneLoader SceneLoader;
        public PacketSpawner PacketSpawner;
        public Player Player;
        public InputManager InputManager;
		public PortLoader PortLoader;
        public BackgroundMusic BackgroundMusic;
        public Room Room;

        public static GameScore Score;
        public static Scoreboard Scoreboard;

        public static LevelParameters LevelParameters;

        public static bool IsGameOver;
        public static bool IsPaused;

        public static string PacketSinksPath = "/Sinks";
        public static string PacketSourcesPath = "/Sources";

        private void Start()
        {
            Initialize();
        }

        public void LoadLevelData()
        {
            // TODO
			LevelParameters.LevelName = "default_level";
            LevelParameters.NumDroppedPacketsAllowed = 5;
            LevelParameters.BackgroundSoundtrack = Soundtrack.DeepDreamMachine;
        }

        public void LoadPorts()
        {
            if (PortLoader != null)
            {
                PortLoader.Initialize(LevelParameters);
            }
            else
            {
                Debug.Log("No PortLoader found. Skipping initialization.");
            }

            // Try to find Sinks and Sources gameobject in scene, if not already set.
            if (PacketSinks == null)
            {
                PacketSinks = GameObject.Find(PacketSinksPath);
            }

            if (PacketSources == null)
            {
                PacketSources = GameObject.Find(PacketSourcesPath);
            }

            // If we found the Sinks/Sources obj, load their children as sources and sinks.
            if (PacketSources != null)
            {
                AllPacketSources = new List<PacketSource>(PacketSources.GetComponentsInChildren<PacketSource>());

                foreach (PacketSource source in AllPacketSources)
                {
                    source.Initialize();
                }
            }

            if (PacketSinks != null)
            {
                AllPacketSinks = new List<PacketSink>(PacketSinks.GetComponentsInChildren<PacketSink>());
            }
        }

        public void Initialize()
        {
            LoadLevelData();

            if (SceneLoader == null)
            {
                SceneLoader = gameObject.AddComponent<SceneLoader>();
                SceneLoader.Initialize();
            }

            if (InputManager != null)
            {
                InputManager.Initialize();
            }
            else
            {
                Debug.Log("No InputManager found. Skipping initialization.");
            }

            LoadPorts();

            if (Player != null)
            {
                Player.Initialize();
            }
            else
            {
                Debug.Log("No Player found. Skipping initialization.");
            }

            if (PacketSpawner != null)
            {
                PacketSpawner.Initialize(this);
            }

            foreach (PacketSink sink in AllPacketSinks)
            {
                sink.Initialize();
            }

            ResetScore(ref Score);

            if (Scoreboard == null)
            {
                var scoreboard = GameObject.Find("[Scoreboard]");
                if (scoreboard != null)
                {
                    Scoreboard = scoreboard.GetComponent<Scoreboard>();
                    Scoreboard.Initialize(Score);
                }
            }

            if (BackgroundMusic != null)
            {
                BackgroundMusic.Initialize();

                BackgroundMusic.SetBackgroundSoundtrack(
                    LevelParameters.BackgroundSoundtrack);
            }

            if (Room != null)
            {
                Room.Initialize(BackgroundMusic.BackgroundMusicSource);
            }
        }

        public static void ReportPacketDelivered(Packet p)
        {
            Score.PacketsDelivered++;
            Score.BytesDelivered += p.Size;
        }

        public static void ReportVirusDelivered(Virus v)
        {
            Score.VirusAmount += v.Damage;
            Score.NumberOfVirusesInfected++;
        }

        public static void ReportPacketDropped(Packet p)
        {
            Score.PacketsDropped++;
        }

        public static void ReportStoppedVirus(Virus v)
        {
            Score.NumberOfVirusesStopped++;
        }

        public void ResetScore(ref GameScore score)
        {
            score.BytesDelivered = 0;
            score.PacketsDelivered = 0;
            score.PacketsDropped = 0;
            score.Time = 0.0f;
            score.NumberOfVirusesInfected = 0;
            score.NumberOfVirusesStopped = 0;
        }

        private void GameOver()
        {
            IsGameOver = true;
            Debug.Log("Time: " + Score.Time + "  Number of packets delivered: " + Score.PacketsDelivered);

            SceneLoader.TransitionToScene("MainMenu");
        }

        public void Update()
        {
            if (!IsPaused)
            {
                Score.Time += Time.deltaTime;

                if (Scoreboard != null)
                {
                    Scoreboard.UpdateScore(Score);
                }

                if (Score.PacketsDropped > LevelParameters.NumDroppedPacketsAllowed && !IsGameOver)
                {
                    GameOver();
                }
            }
        }
    }
}
