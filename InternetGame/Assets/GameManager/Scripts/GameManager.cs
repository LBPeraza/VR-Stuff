using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class GameManager : MonoBehaviour
    {
        public List<PacketSource> PacketSources;
        public List<PacketSink> PacketSinks;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (PacketSources[0].QueuedPackets.Count == 0)
            {
                Packet p = PacketFactory.CreateEmail(PacketSources[0], PacketSinks[0]);
            }
        }
    }
}
