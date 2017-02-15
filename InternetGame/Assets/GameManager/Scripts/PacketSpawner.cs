using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSpawner : MonoBehaviour
    {
        public List<PacketSource> Sources;
        public List<PacketSink> Sinks;

        public GameManager GameManager;

        public static Hashtable AddressToColor;

        private static Color[] AddressColors = { Color.green, Color.blue, Color.magenta, Color.yellow };

        public virtual void Initialize(GameManager manager)
        {
            GameManager = manager;
            Sources = GameManager.AllPacketSources;
            Sinks = GameManager.AllPacketSinks;

            AddressToColor = new Hashtable();
            BuildAddressToColorTable(Sinks);
        }

        public void BuildAddressToColorTable(List<PacketSink> Sinks)
        {
            int i = 0;
            foreach (var sink in Sinks)
            {
                if (!AddressToColor.ContainsKey(sink.Address))
                {
                    AddressToColor.Add(sink.Address, AddressColors[i]);
                    i++;
                }
            }
        }

   
        // Update is called once per frame
        public virtual void Update()
        {
            
        }
    }
}

