using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketFactory
    {
        public static Packet CreateEmail()
        {
            GameObject emailContainer = new GameObject("Email");
            Email email = emailContainer.AddComponent<Email>();

            email.Initialize();

            return email;
        }
    }
}
