using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkFactory
    {
        public static float SEGMENT_ADD_INTERVAL = 0.03f; // seconds
        public static float LINK_MIN_LENGTH = 0.05f; // meters
        public static float LINK_BANDWIDTH = 200.0f;
        public static bool USE_CYLINDRICAL_LINK_SEGMENT = false;

        public static GameObject CreateLink(PacketSource s)
        {
            GameObject linkContainer = new GameObject("Link");
            Link link = linkContainer.AddComponent<SplittingLink>();

            link.Source = s;
            link.SegmentAddInterval = SEGMENT_ADD_INTERVAL;
            link.SegmentMinLength = LINK_MIN_LENGTH;
            link.Bandwidth = LINK_BANDWIDTH;

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

