using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace InternetGame
{
    public class MainMenuLevelController : LevelController
    {
        // TODO
        Dictionary<string, MenuOption> Levels;
        Dictionary<PacketSource, MenuOption> PortAssociations;

        struct MenuOption
        {
            public string SceneName;
            public LevelParameters LevelParameters;
        }

        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            Levels = new Dictionary<string, MenuOption>
            {
                {
                    "Infinite Mode", new MenuOption {
                        SceneName = "InfiniteLevel",
                        LevelParameters = LevelParameters.LoadFromFile("infinite_level")
                    }
                },
                {
                    "Level One",new MenuOption {
                        SceneName = "WaveLevel",
                        LevelParameters = LevelParameters.LoadFromFile("level_one")
                    }
                },
                {
                    "Level Two",new MenuOption {
                        SceneName = "WaveLevel",
                        LevelParameters = LevelParameters.LoadFromFile("level_two")
                    }
                }
            };
            PortAssociations = new Dictionary<PacketSource, MenuOption>();

            AssociatePortsWithLevels();
            SpawnPacketsAtLevelPorts();
        }

        private void AssociatePortsWithLevels()
        {
            int i = 0;
            foreach (var valuePair in Levels)
            {
                if (i > Sources.Count - 1)
                {
                    Debug.Log("Ran out of ports in MainMenuLevelController");
                    break;
                }

                Sources[i].OnPendingLinkStarted += OnLinkStarted;
                Text textDisplay = Sources[i].transform.GetComponentInChildren<Text>();
                if (textDisplay != null)
                {
                    textDisplay.text = valuePair.Key;
                }

                // Associate level name with the port.
                PortAssociations[Sources[i]] = valuePair.Value;

                i++;
            }
        }

        private void OnLinkStarted(object sender, LinkEventArgs l)
        {
            MenuOption picked = PortAssociations[l.Link.Source];
            if (picked.LevelParameters != null)
            {
                picked.LevelParameters.transform.parent = null;
                picked.LevelParameters.name = "LevelParameters";

                DontDestroyOnLoad(picked.LevelParameters.gameObject);
            }
            LoadLevel(picked.SceneName);
        }

        private void LoadLevel(string levelName)
        {
            GameManager.SceneLoader.TransitionToScene(levelName);
        }

        private void SpawnPacketsAtLevelPorts()
        {
            int i = 0;
            foreach (var valuePair in PortAssociations)
            {
                PacketSource port = valuePair.Key;
                Packet packet = PacketFactory.CreateEmail(port, Sinks[i % Sinks.Count]);
                packet.Patience = float.MaxValue;
                packet.AlertTime = float.MaxValue;

                i++;
            }
        }

        public override void Update()
        {
            base.Update();

        }
    }
}

