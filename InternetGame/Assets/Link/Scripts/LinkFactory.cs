using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class LinkFactory
    {
        public static float SEGMENT_ADD_INTERVAL = 0.1f;
        public static GameObject CreateLink()
        {
            GameObject linkContainer = new GameObject("Link");
            Link link = linkContainer.AddComponent<Link>();

            link.SegmentAddInterval = SEGMENT_ADD_INTERVAL;
            link.LinkSegmentPrefab = (GameObject) Resources.Load("LinkSegment");

            return linkContainer;
        }
    }
}

