using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK
{

    public class PacketDropZone : VRTK_SnapDropZone
    {
        public float StraightenRate = 0.1f;
        public float IntertiaRate = 0.5f;

        private void VisualizeCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            LineRenderer line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.startWidth = 0.01f;
            line.endWidth = 0.01f;
            line.numPositions = 4;
            line.SetPositions(new Vector3[4] { p0, p1, p2, p3 });
        }

        protected override IEnumerator UpdateTransformDimensions(VRTK_InteractableObject ioCheck, GameObject endSettings, Vector3 endScale, float duration)
        {
            var elapsedTime = 0f;
            var ioTransform = ioCheck.transform;
            var startPosition = ioTransform.position;
            var distance = Vector3.Distance(endSettings.transform.position, startPosition);
            InternetGame.Connector connector = ioCheck.GetComponentInParent<InternetGame.Connector>();
            var link = connector.Link;
            var p1 = startPosition + (link.RecentSlope.normalized * distance * IntertiaRate);
            var p2 = endSettings.transform.position + (endSettings.transform.forward * -distance * StraightenRate);
            var startRotation = ioTransform.rotation;
            var startScale = ioTransform.localScale;
            var storedKinematicState = ioCheck.isKinematic;
            ioCheck.isKinematic = true;

            // VisualizeCurve(startPosition, p1, p2, endSettings.transform.position);

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                ioTransform.position = InternetGame.Link.CubicLerp(
                    startPosition, 
                    p1,
                    p2,
                    endSettings.transform.position,
                    (elapsedTime / duration));
                ioTransform.rotation = Quaternion.Lerp(startRotation, endSettings.transform.rotation, (elapsedTime / duration));
                ioTransform.localScale = Vector3.Lerp(startScale, endScale, (elapsedTime / duration));
                yield return null;
            }

            //Force all to the last setting in case anything has moved during the transition
            ioTransform.position = endSettings.transform.position;
            ioTransform.rotation = endSettings.transform.rotation;
            ioTransform.localScale = endScale;

            ioCheck.isKinematic = storedKinematicState;
            SetDropSnapType(ioCheck);
        }

    }
}
