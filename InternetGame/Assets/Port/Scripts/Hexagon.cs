using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Hexagon : MonoBehaviour
    {
        public float Radius;
        public float RingThickness = 0.15f;
        public GameObject HexagonSidePrefab;

        public void Initialize(float radius, float thickness)
        {
            Radius = radius;
            RingThickness = thickness;

            HexagonSidePrefab = Resources.Load<GameObject>("HexagonSide");

            var rRoot3Over4 = radius * Mathf.Sqrt(3) / 4.0f;
            var threeFourthsR = 3.0f * radius / 4.0f;
            var rRoot3Over2 = radius * Mathf.Sqrt(3) / 2.0f;

            // Left upper.
            var leftUpper = Instantiate(HexagonSidePrefab, this.transform, false);
            leftUpper.transform.localRotation = Quaternion.Euler(0, 0, -30.0f);
            leftUpper.transform.localPosition = new Vector3(-threeFourthsR, rRoot3Over4, 0.0f);

            // Left lower.
            var leftLower = Instantiate(HexagonSidePrefab, this.transform, false);
            leftLower.transform.localRotation = Quaternion.Euler(0, 0, 30.0f);
            leftLower.transform.localPosition = new Vector3(-threeFourthsR, -rRoot3Over4, 0.0f);

            // Right upper.
            var rightUpper = Instantiate(HexagonSidePrefab, this.transform, false);
            rightUpper.transform.localRotation = Quaternion.Euler(0, 0, 30.0f);
            rightUpper.transform.localPosition = new Vector3(threeFourthsR, rRoot3Over4, 0.0f);

            // Right lower.
            var rightLower = Instantiate(HexagonSidePrefab, this.transform, false);
            rightLower.transform.localRotation = Quaternion.Euler(0, 0, -30.0f);
            rightLower.transform.localPosition = new Vector3(threeFourthsR, -rRoot3Over4, 0.0f);

            // Top.
            var top = Instantiate(HexagonSidePrefab, this.transform, false);
            top.transform.localRotation = Quaternion.Euler(0, 0, 90.0f);
            top.transform.localPosition = new Vector3(0, rRoot3Over2, 0);

            // Bottom.
            var bottom = Instantiate(HexagonSidePrefab, this.transform, false);
            bottom.transform.localRotation = Quaternion.Euler(0, 0, 90.0f);
            bottom.transform.localPosition = new Vector3(0, -rRoot3Over2, 0);

            foreach (Transform t in this.transform)
            {
                t.localScale = new Vector3(RingThickness, radius, RingThickness);
            }
        }
     
        public void SetMaterial(Material m)
        {
            foreach (Transform t in this.transform)
            {
                t.GetComponent<Renderer>().material = m;
            }
        }   
    }
}

