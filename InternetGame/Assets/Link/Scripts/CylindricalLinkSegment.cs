using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class CylindricalLinkSegment : LinkSegment
    {
        public GameObject Cap;
        public GameObject Cylinder;

        private SkinnedMeshRenderer cylinderRenderer;

        public override void Initialize()
        {
            base.Initialize();

            cylinderRenderer = Cylinder.GetComponent<SkinnedMeshRenderer>();
        }

        public override void SetSize(float thickness, float maxThickness, float length)
        {
            base.SetSize(maxThickness, maxThickness, length);

            // Update cap depth specially.
            Cap.transform.localScale = new Vector3(thickness / maxThickness, thickness / maxThickness, thickness / length);
        }

        public override void SetBetween(Vector3 from, Vector3 to, float segmentThickness, float maxThickness,
                                        float segmentLength = -1.0f, float segmentThicknessStart = -1.0f)
        {
            base.SetBetween(from, to, segmentThickness, maxThickness, segmentLength, segmentThicknessStart);

            float endPercentThickness = (segmentThickness / maxThickness) * 100.0f;
            cylinderRenderer.SetBlendShapeWeight(0, endPercentThickness);
            if (segmentThicknessStart > 0)
            {
                float startPercentThickness = (segmentThicknessStart / maxThickness) * 100.0f;
                cylinderRenderer.SetBlendShapeWeight(1, startPercentThickness);
            }
            else
            {
                cylinderRenderer.SetBlendShapeWeight(1, endPercentThickness);
            }
        }

    }

}
