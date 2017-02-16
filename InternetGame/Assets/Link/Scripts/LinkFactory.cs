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

        public static GameObject CreateLink(PacketSource s)
        {
            GameObject linkContainer = new GameObject("Link");
            Link link = linkContainer.AddComponent<SplittingLink>();

            link.Source = s;
            link.SegmentAddInterval = SEGMENT_ADD_INTERVAL;
            link.SegmentMinLength = LINK_MIN_LENGTH;
            link.Bandwidth = LINK_BANDWIDTH;
			link.UntaperedDiameter = LINK_THICKNESS;

            if (USE_CYLINDRICAL_LINK_SEGMENT)
            {
                link.LinkSegmentPrefab = (GameObject)Resources.Load("Prefabs/CylindricalLinkSegment");
            }
            else
            {
                link.LinkSegmentPrefab = (GameObject)Resources.Load("Prefabs/LinkSegment");
            }

            return linkContainer;
        }
    }
}

