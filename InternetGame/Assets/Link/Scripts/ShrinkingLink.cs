using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class ShrinkingLink : Link
    {
        private bool isAnimatingDestruction;

        public override void AnimateAndDestroy(SeverCause cause, LinkSegment linkSegment)
        {
            base.AnimateAndDestroy(cause, linkSegment);

            isAnimatingDestruction = true;
            var fadeCoroutine = Fade();
            StartCoroutine(fadeCoroutine);
        }

        private IEnumerator Fade()
        {
            float originalThickness = Segments.Count > 0 ? Segments[0].transform.localScale.x : .03f;
            for (float f = originalThickness; f >= 0; f -= 0.001f)
            {
                foreach (LinkSegment segment in Segments)
                {
                    segment.transform.localScale = new Vector3(f, f, segment.transform.localScale.z);
                }
                yield return null;
            }

            Destroy(this.gameObject);
        }

    }
}
