using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketFactory
    {
        public static float DEFAULT_PACKET_PATIENCE = 15.0f; // Seconds.
        public static float DEFAULT_PACKET_ALERT_TIME = 10.0f; // Seconds

        public static Packet CreateEmail(PacketSource s, PacketSink t)
        {
            GameObject emailContainer = new GameObject("Email");
            emailContainer.transform.parent = s.transform;

            Email email = emailContainer.AddComponent<Email>();
            email.Destination = t.Address;
            email.Patience = DEFAULT_PACKET_PATIENCE;
            email.AlertTime = DEFAULT_PACKET_ALERT_TIME;

            email.Initialize();

            s.EnqueuePacket(email);

            return email;
        }

        public static Virus CreateEmailVirus(PacketSource s, PacketSink t)
        {
            GameObject emailContainer = new GameObject("EmailVirus");
            emailContainer.transform.parent = s.transform;

            ChameleonVirus emailVirus = emailContainer.AddComponent<ChameleonVirus>();
            emailVirus.Destination = t.Address;
            emailVirus.Patience = DEFAULT_PACKET_PATIENCE;
            emailVirus.AlertTime = DEFAULT_PACKET_ALERT_TIME;
            emailVirus.ColorChangePercentageOffset = 0.1f; // 10%

            emailVirus.Initialize();

            s.EnqueuePacket(emailVirus);

            return emailVirus;
        }
    }
}
