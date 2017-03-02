using System;
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

    public class GameManager : MonoBehaviour, ResourceLoadable
    {
        public GameObject PacketSources;
        public GameObject PacketSinks;

        public static List<PacketSource> AllPacketSources;
        public static List<PacketSink> AllPacketSinks;

        public SceneLoader SceneLoader;
        public PacketSpawner PacketSpawner;
        public Player Player;
        public GameObject HeadCamera;
        public InputManager InputManager;
		public PortLoader PortLoader;
        public BackgroundMusic BackgroundMusic;
        public Room Room;
        public GameOverOptions GameOverOptions;
        public GameScore Score;
        public Scoreboard Scoreboard;

        public string LevelName;

        [HideInInspector]
        public LevelParameters LevelParameters;

        public bool IsGameOver;
        public bool IsPaused;

        public static string PacketSinksPath = "/Sinks";
        public static string PacketSourcesPath = "/Sources";

        protected static GameManager instance;

        private void Start()
        {
            SetInstance(this);
            Initialize();
        }

        public static void SetInstance(GameManager manager)
        {
            instance = manager;
        }

        public static GameManager GetInstance()
        {
            return instance;
        }

        public void LoadLevelData()
        {
            // TODO
            var levelParamsContainer = GameObject.Find("/LevelParameters");
            if (levelParamsContainer != null)
            {
                LevelParameters = levelParamsContainer.GetComponent<LevelParameters>();
                LevelName = LevelParameters.LevelName;
            }
            if (LevelParameters == null)
            {
                Debug.LogWarning("No LevelParameter object found in scene, loading one manually.");
                LevelParameters = LevelParameters.LoadFromFile(LevelName);
            }
        }

        public void LoadPorts()
        {
            if (PortLoader == null)
            {
                PortLoader = gameObject.AddComponent<PortLoader>();
                PortLoader.LoadFrom = LoadLocation.NoLoad;
            }
            PortLoader.Initialize(LevelParameters);

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

        public void LoadResources()
        {
        }

        public void InitializeFactories()
        {
            PacketFactory.Initialize();
            ConnectorFactory.Initialize();
            LinkFactory.Initialize();
        }

        public void Initialize()
        {
            LoadResources();

            LoadLevelData();

            InitializeFactories();

            if (HeadCamera == null)
            {
                HeadCamera = GameObject.FindWithTag("MainCamera");
            }

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

            GameUtils.Initialize(this);

            if (Player != null)
            {
                Player.Initialize();
            }
            else
            {
                Debug.Log("No Player found. Skipping initialization.");
            }

            if (PacketSpawner == null)
            {
                switch (LevelParameters.PacketSpawner)
                {
                    case PacketSpawnerType.Infinite:
                        PacketSpawner = gameObject.AddComponent<InfinitePacketSpawner>();
                        break;
                    case PacketSpawnerType.MainMenu:
                        PacketSpawner = gameObject.AddComponent<MainMenuPacketSpawner>();
                        break;
                    case PacketSpawnerType.Wave:
                        PacketSpawner = gameObject.AddComponent<LevelPacketSpawner>();
                        break;
                }
            }
            PacketSpawner.Initialize(this);

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

            if (BackgroundMusic == null)
            {
                BackgroundMusic = gameObject.AddComponent<BackgroundMusic>();
            }
            BackgroundMusic.Initialize();

            BackgroundMusic.SetBackgroundSoundtrack(
                LevelParameters.BackgroundSoundtrack);

            if (Room != null)
            {
                Room.Initialize(BackgroundMusic.BackgroundMusicSource);
            }

            if (GameOverOptions != null)
            {
                GameOverOptions.Initialize();
            }
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            if (IsPaused)
            {
                //Time.timeScale = 0.0f;
                BackgroundMusic.Pause();

                GameOverOptions.Show();
            }
            else
            {
                //Time.timeScale = 1.0f;
                BackgroundMusic.Resume();

                GameOverOptions.Hide();
            }
        }

        public void ReportPacketDelivered(Packet p)
        {
            Score.PacketsDelivered++;
            Score.BytesDelivered += p.Payload.Size;
        }

        public void ReportVirusDelivered(Virus v)
        {
            Score.VirusAmount += v.Damage;
            Score.NumberOfVirusesInfected++;
            Score.PacketsDropped++;
        }

        public void ReportPacketDropped(Packet p)
        {
            Score.PacketsDropped++;
        }

        public void ReportStoppedVirus(Virus v)
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

            TogglePause();
        }

        public void LevelClearead()
        {
            // TODO
            GameOver();
        }

        public void Quit()
        {
            SceneLoader.TransitionToScene("MainMenu");
        }

        public void Retry()
        {
            SceneLoader.TransitionToScene("LevelOne");
        }

        public float GameTime()
        {
            return Score.Time;
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
