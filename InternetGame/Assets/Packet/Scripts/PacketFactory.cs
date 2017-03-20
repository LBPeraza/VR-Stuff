using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public enum PacketMode
    {
        FallingPackets,
        TimedOnDeck
    }

    public enum PacketPayloadType
    {
        Email,
        ChameleonVirus,
        Netflix
    }

    public class PacketFactory
    {
        public static float DEFAULT_PACKET_PATIENCE = 17.0f; // Seconds.
        public static float DEFAULT_PACKET_ALERT_TIME = 10.0f; // Seconds

        public static PacketMode Mode = PacketMode.TimedOnDeck;

        public static void LoadResources()
        {
            Email.LoadResources();
            Netflix.LoadResources();
            Virus.LoadResources();
            ChameleonVirus.LoadResources();
        }

        public static void Initialize()
        {
            LoadResources();
        }

        public static Packet CreatePacket(PacketSource s, PacketSink t)
        {
            GameObject emailContainer = new GameObject("Packet");
            emailContainer.transform.parent = s.PacketContainer.transform;

            Packet packet = null;
            switch (Mode)
            {
                case PacketMode.FallingPackets:
                    packet = emailContainer.AddComponent<FallingPacket>();
                    break;
                case PacketMode.TimedOnDeck:
                    packet = emailContainer.AddComponent<TimedPacket>();
                    break;
            }

            packet.Destination = t.Address;
            packet.Patience = DEFAULT_PACKET_PATIENCE;
            packet.AlertTime = DEFAULT_PACKET_ALERT_TIME;

            return packet;
        }

        public static Packet CreateEmail(PacketSource s, PacketSink t)
        {
            Packet p = CreatePacket(s, t);
            p.Payload = p.gameObject.AddComponent<Email>();

            p.Initialize();

            s.EnqueuePacket(p);

            return p;
        }

        public static Packet CreateEmailVirus(PacketSource s, PacketSink t)
        {
            Packet p = CreatePacket(s, t);
            p.Payload = p.gameObject.AddComponent<ChameleonVirus>();

            ChameleonVirus emailVirus = (ChameleonVirus) p.Payload;
            emailVirus.ColorChangePercentageOffset = 0.1f; // 10%
            emailVirus.VirusAlertPercentage = 0.6f; // 60%

            p.Initialize();

            s.EnqueuePacket(p);

            return p;
        }

        public static Packet CreateLoadedPacket(
            PacketSource s,
            PacketSink t, 
            PacketPayloadType payloadType)
        {
            Packet p = CreatePacket(s, t);

            switch (payloadType)
            {
                case PacketPayloadType.ChameleonVirus:
                    p.Payload = p.gameObject.AddComponent<ChameleonVirus>();
                    ChameleonVirus emailVirus = (ChameleonVirus)p.Payload;
                    emailVirus.ColorChangePercentageOffset = 0.1f; // 10%
                    emailVirus.VirusAlertPercentage = 0.6f; // 60%
                    break;
                case PacketPayloadType.Email:
                    p.Payload = p.gameObject.AddComponent<Email>();
                    break;
                case PacketPayloadType.Netflix:
                    p.Payload = p.gameObject.AddComponent<Netflix>();
                    break;
            }
            p.Initialize();

            s.EnqueuePacket(p);

            return p;
        }
    }
}
