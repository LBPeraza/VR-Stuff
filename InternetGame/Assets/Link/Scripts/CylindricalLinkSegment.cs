using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class CylindricalLinkSegment : LinkSegment
    {
        public GameObject Cap;
		public GameObject Cylinder;

		private SkinnedMeshRenderer renderer;

        public override void Initialize()
        {
            base.Initialize();

			renderer = Cylinder.GetComponent<SkinnedMeshRenderer> ();
        }

		public override void SetSize(float thickness, float length) {
			base.SetSize (thickness, length);
			Cap.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f / length);
		}

        public override void SetBetween(Vector3 from, Vector3 to, float segmentThickness, float maxThickness,
										float segmentLength = -1.0f, float segmentThicknessStart = -1.0f)
        {
            base.SetBetween(from, to, segmentThickness, segmentLength, segmentThicknessStart);

			float percent = segmentThickness / maxThickness;
			renderer.SetBlendShapeWeight (0, percent);
			if (segmentThicknessStart > 0)
				renderer.SetBlendShapeWeight (1, segmentThicknessStart / maxThickness);
			else
				renderer.SetBlendShapeWeight (1, percent);
        }

    }

}
