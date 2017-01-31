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
    }

    public class GameManager : MonoBehaviour
    {
        public List<PacketSource> PacketSources;
        public List<PacketSink> PacketSinks;

        public static List<PacketSource> AllPacketSources;
        public static List<PacketSink> AllPacketSinks;

        public Player Player;
        public InputManager InputManager;

        public GameScore Score;

        // Use this for initialization
        void Start()
        {
            Initialize();

            InputManager.Initialize();
            Player.Initialize();

            ResetScore(Score);
        }

        public void Initialize()
        {
            AllPacketSources = PacketSources;
            AllPacketSinks = PacketSinks;
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
            if (PacketSources.Count > 0 && PacketSinks.Count > 0)
            {
                if (PacketSources[0].QueuedPackets.Count == 0)
                {
                    Packet p = PacketFactory.CreateEmail(PacketSources[0], PacketSinks[0]);
                }
            }
        }
    }
}
