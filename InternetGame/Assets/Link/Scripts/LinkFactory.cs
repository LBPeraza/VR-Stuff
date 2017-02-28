using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkFactory
    {
        public static float SEGMENT_ADD_INTERVAL = 0.03f; // seconds
        public static float LINK_MIN_LENGTH = 0.02f; // meters
        public static float LINK_BANDWIDTH = 200.0f;
        public static bool USE_CYLINDRICAL_LINK_SEGMENT = true;
		public static float LINK_THICKNESS = 0.03f;
		public static float LINK_TAPERED_THICKNESS = 0.01f;
        public static float BASE_PACKET_SIZE = 1400.0f;

        public static GameObject CylindricalLinkSegmentPrefab;
        public static GameObject SquareLinkSegmentPrefab; 

        public static void LoadResources()
        {
            CylindricalLinkSegmentPrefab = (GameObject)Resources.Load("Prefabs/CylindricalLinkSegment");
            SquareLinkSegmentPrefab = (GameObject)Resources.Load("Prefabs/LinkSegment");

            SplittingLink.LoadResources();
        }

        public static void Initialize()
        {
            LoadResources();
        }

        public static float PayloadThickness(PacketPayload p)
        {
            return (p.Size / BASE_PACKET_SIZE) * LINK_THICKNESS;
        }

        public static GameObject CreateLink(PacketSource s, Packet p)
        {
            GameObject linkContainer = new GameObject("Link");
            Link link = linkContainer.AddComponent<SplittingLink>();

            link.Source = s;
            link.SegmentAddInterval = SEGMENT_ADD_INTERVAL;
            link.SegmentMinLength = LINK_MIN_LENGTH;
            link.Bandwidth = LINK_BANDWIDTH;
			link.UntaperedDiameter = PayloadThickness(p.Payload);

            if (USE_CYLINDRICAL_LINK_SEGMENT)
            {
                link.LinkSegmentPrefab = CylindricalLinkSegmentPrefab;
            }
            else
            {
                link.LinkSegmentPrefab = SquareLinkSegmentPrefab;
            }

            return linkContainer;
        }
    }
}

