using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketFactory
    {
        public static Packet CreateEmail(PacketSource s, PacketSink t)
        {
            GameObject emailContainer = new GameObject("Email");
            emailContainer.transform.parent = s.transform;

            Email email = emailContainer.AddComponent<Email>();
            email.Destination = t.Address;

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
            emailVirus.ColorChangePercentageOffset = 0.1f; // 10%

            emailVirus.Initialize();

            s.EnqueuePacket(emailVirus);

            return emailVirus;
        }
    }
}
