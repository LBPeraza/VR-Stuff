using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class GameUtils
    {
        public static Hashtable AddressToColor;
        public static Hashtable AddressToSink;
        public static Hashtable AddressToSource;

        private static Color[] AddressColors = { Color.green, Color.blue, Color.cyan, Color.yellow };

        public static void Initialize(GameManager manager)
        {
            AddressToColor = new Hashtable();
            AddressToSink = new Hashtable();
            AddressToSource = new Hashtable();

            BuildAddressToColorTable(GameManager.AllPacketSinks);
            BuildAddressToSinkTable(GameManager.AllPacketSinks);
            BuildAddressToSourceTable(GameManager.AllPacketSources);
        }

        private static void BuildAddressToColorTable(List<PacketSink> Sinks)
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

        private static void BuildAddressToSinkTable(List<PacketSink> Sinks)
        {
            int i = 0;
            foreach (var sink in Sinks)
            {
                if (!AddressToSink.ContainsKey(sink.Address))
                {
                    AddressToSink.Add(sink.Address, sink);
                    i++;
                }
            }
        }

        private static void BuildAddressToSourceTable(List<PacketSource> Sources)
        {
            int i = 0;
            foreach (var source in Sources)
            {
                if (!AddressToSource.ContainsKey(source.Address))
                {
                    AddressToSource.Add(source.Address, source);
                    i++;
                }
            }
        }

    }

}
