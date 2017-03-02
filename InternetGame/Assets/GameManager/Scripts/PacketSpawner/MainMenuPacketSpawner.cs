using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace InternetGame
{
    public class MainMenuPacketSpawner : PacketSpawner
    {
        // TODO
        public Dictionary<string, string> Levels;
        public Dictionary<PacketSource, string> PortAssociations;

        public override void Initialize(GameManager manager)
        {
            base.Initialize(manager);

            Levels = new Dictionary<string, string>
            {
                {"Infinite Mode", "LevelOne"},
                {"Level One", "WaveLevel" }
            };
            PortAssociations = new Dictionary<PacketSource, string>();

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
                    Debug.Log("Ran out of ports in MainMenuPacketSpawner");
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

        private void OnLinkStarted(Link l)
        {
            LoadLevel(PortAssociations[l.Source]);
        }

        private void LoadLevel(string levelName)
        {
            GameManager.SceneLoader.TransitionToScene(levelName);
        }

        private void SpawnPacketsAtLevelPorts()
        {
            foreach (var valuePair in PortAssociations)
            {
                PacketSource port = valuePair.Key;
                Packet packet = PacketFactory.CreateEmail(port, Sinks[0]);
                packet.Patience = float.MaxValue;
                packet.AlertTime = float.MaxValue;
            }
        }

        public override void Update()
        {
            base.Update();

        }
    }
}

