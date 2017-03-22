using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PacketSourceFactory : MonoBehaviour
    {
        public static PacketSource SourcePrefab;
        public static PacketLoadingBehavior PacketLoadingBehavior =
            PacketLoadingBehavior.PutPacketOnLinkWhenLinkStarted;
        public static MethodOfEntry PacketMethodOfEntry = 
            MethodOfEntry.FallingPacketHopper;

        public static void LoadResources()
        {
            if (SourcePrefab == null)
            {
                SourcePrefab = Resources.Load<PacketSource>("Prefabs/Source");
            }
        }

        public static void Initialize()
        {
            LoadResources();
        }

        public static PacketSource CreatePacketSource(
            Transform parent, 
            Vector3 localPosition, 
            Quaternion localRotation)
        {
            PacketSource src = Instantiate<PacketSource>(
                SourcePrefab, parent);
            src.transform.localPosition = localPosition;
            src.transform.localRotation = localRotation;

            src.PacketLoadingBehavior = PacketLoadingBehavior;
            src.MethodOfEntry = PacketMethodOfEntry;

            return src;
        }

    }
}
