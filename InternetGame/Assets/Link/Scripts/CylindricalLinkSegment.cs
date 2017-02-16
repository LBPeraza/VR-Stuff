using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class CylindricalLinkSegment : LinkSegment
    {
        public GameObject Cap;

        public Vector3 initialScale;

        public override void Initialize()
        {
            base.Initialize();

            initialScale = Cap.transform.localScale;
        }

        public override void SetBetween(Vector3 from, Vector3 to, float segmentThickness, float segmentLength = -1.0f)
        {
            base.SetBetween(from, to, segmentThickness, segmentLength);

            Vector3 v = new Vector3(segmentThickness, segmentThickness, (1.0f / segmentLength));
            Cap.transform.localScale = Vector3.Scale(initialScale, v);
        }

    }

}
