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

    public enum GameManagerSoundEffect
    {
        GameOver,
        LevelCleared
    }

    public class GameManager : MonoBehaviour, ResourceLoadable
    {
        [Header("Port Configuration")]
        public GameObject PacketSources;
        public GameObject PacketSinks;
        public PortLoader PortLoader;

        [Header("Player Configuration")]
        public Player Player;

        [Header("Room Configuration")]
        public Room Room;

        [Header("Game Options")]
        public GameOverOptions GameOverOptions;
        public NameEntry NameEntry;
        public string LevelName;
        public GameObject DebugOptions;
        public bool EnableDebug;

        private bool debugDisplayEnabled = false;
        public bool DebugDisplayEnabled
        {
            get { return debugDisplayEnabled; }
            set
            {
                debugDisplayEnabled = value;
                if (debugDisplayEnabled && DebugOptions)
                {
                    DebugOptions.SetActive(true);
                }
                else if (!debugDisplayEnabled && DebugOptions)
                {
                    DebugOptions.SetActive(false);
                }
            }
        }

        [HideInInspector]
        public LevelParameters LevelParameters;

        [HideInInspector]
        public bool IsGameOver;
        [HideInInspector]
        public bool IsPaused;
        [HideInInspector]
        public SceneLoader SceneLoader;
        [HideInInspector]
        public GameObject HeadCamera;

        public static List<PacketSource> AllPacketSources;
        public static List<PacketSink> AllPacketSinks;

		public static List<TextSpawner> AllTextSpawners;

        public static string PacketSinksPath = "/Sinks";
        public static string PacketSourcesPath = "/Sources";

        protected static GameManager instance;

        protected LevelController LevelController;

        [HideInInspector]
        public GameScore Score;
        protected Scoreboard Scoreboard;

        protected BackgroundMusic BackgroundMusic;
        protected AudioSource GameOverSource;
        protected AudioSource LevelCompletedSource;
        protected AudioClip LevelCompletedClip;
        protected AudioClip GameOverClip;
        protected NameEntry NameEntryPrefab;

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
            GameOverClip = Resources.Load<AudioClip>("Audio/gameover");
            LevelCompletedClip = Resources.Load<AudioClip>("Audio/level_completed");

            NameEntryPrefab = Resources.Load<NameEntry>("Prefabs/NameEntry");
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

            if (LevelController == null)
            {
                switch (LevelParameters.LevelController)
                {
                    case LevelControllerType.Infinite:
                        LevelController = gameObject.AddComponent<InfiniteLevelController>();
                        break;
                    case LevelControllerType.MainMenu:
                        LevelController = gameObject.AddComponent<MainMenuLevelController>();
                        break;
                    case LevelControllerType.Wave:
                        LevelController = gameObject.AddComponent<WaveLevelController>();
                        break;
                }
            }
            LevelController.Initialize(this);

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

            GameOverSource = AudioMix.Add2DAudioSourceTo(this.gameObject);
            LevelCompletedSource = AudioMix.Add2DAudioSourceTo(this.gameObject);

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

            if (NameEntry != null)
            {
                NameEntry.Initialize();
            }

             DebugDisplayEnabled = EnableDebug;
        }

        public void PlayClip(GameManagerSoundEffect effect)
        {
            AudioSource source = GameOverSource;
            AudioClip clip = GameOverClip;
            bool loop = false;
            float volume = 1.0f;

            switch (effect)
            {
                case GameManagerSoundEffect.GameOver:
                    clip = GameOverClip;
                    source = GameOverSource;
                    volume = 1.0f;
                    break;
                case GameManagerSoundEffect.LevelCleared:
                    clip = LevelCompletedClip;
                    source = LevelCompletedSource;
                    volume = 1.0f;
                    break;
            }

            source.clip = clip;
            source.volume = volume;
            source.loop = loop;
            source.Play();
        }

        public void TogglePause(bool pauseMusic = true)
        {
            IsPaused = !IsPaused;
            if (IsPaused)
            {
                if (pauseMusic)
                {
                    BackgroundMusic.Pause();
                }

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

            PlayClip(GameManagerSoundEffect.GameOver);

            BackgroundMusic.SetBackgroundSoundtrack(Soundtrack.Gameover);
            TogglePause(false /* pause music */);

            foreach (PacketSource source in AllPacketSources)
            {
                source.OnGameOver();
            }

            if (NameEntry != null)
            {
                NameEntry.Show();
            }
        }

        public void LevelCleared()
        {
            PlayClip(GameManagerSoundEffect.LevelCleared);

            foreach (PacketSource source in AllPacketSources)
            {
                source.OnLevelCleared();
            }

            TogglePause();
        }

        public void Quit()
        {
            if (LevelParameters.gameObject)
            {
                DestroyImmediate(LevelParameters.gameObject);

                SceneLoader.TransitionToScene("MainMenu");
            }
        }

        public void Retry()
        {
            SceneLoader.TransitionToScene(SceneManager.GetActiveScene().name);
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
