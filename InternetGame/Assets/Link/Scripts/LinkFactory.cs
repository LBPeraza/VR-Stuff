using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkFactory
    {
        public static float SEGMENT_ADD_INTERVAL = 0.05f;
        public static float LINK_BANDWIDTH = 200.0f;

        public static GameObject CreateLink(PacketSource s)
        {
            GameObject linkContainer = new GameObject("Link");
            Link link = linkContainer.AddComponent<Link>();

            link.Source = s;
            link.SegmentAddInterval = SEGMENT_ADD_INTERVAL;
            link.Bandwidth = LINK_BANDWIDTH;
            link.LinkSegmentPrefab = (GameObject) Resources.Load("LinkSegment");
            

            return linkContainer;
        }
    }
}

