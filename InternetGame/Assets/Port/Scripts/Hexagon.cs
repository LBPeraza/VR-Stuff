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
        public Packet AssociatedPacket;

        public GameObject RightUpper;
        public GameObject RightLower;
        public GameObject LeftUpper;
        public GameObject LeftLower;
        public GameObject Top;
        public GameObject Bottom;

        public bool IsFlashing;
        public float FlashRate;
        public Color OriginalColor;
        public float RingColorChangeDuration = 1.0f;

        public Material ActiveMaterial;
        public Material NeutralMaterial;
        public bool IsActive;

        private Coroutine colorChangingCoroutine;
        private Coroutine resizingCoroutine;

        public void Initialize(Material neutralMaterial, Material activeMaterial, float radius, float thickness)
        {
            Radius = radius;
            RingThickness = thickness;
            FlashRate = 1.0f;

            NeutralMaterial = new Material(neutralMaterial);
            ActiveMaterial = new Material(activeMaterial);

            HexagonSidePrefab = Resources.Load<GameObject>("HexagonSide");

            RightUpper = Instantiate(HexagonSidePrefab, this.transform, false);
            RightLower = Instantiate(HexagonSidePrefab, this.transform, false);
            LeftUpper = Instantiate(HexagonSidePrefab, this.transform, false);
            LeftLower = Instantiate(HexagonSidePrefab, this.transform, false);
            Top = Instantiate(HexagonSidePrefab, this.transform, false);
            Bottom = Instantiate(HexagonSidePrefab, this.transform, false);

            SetRadius(radius);

            OriginalColor = neutralMaterial.color;
            SetMaterial(neutralMaterial);
        }

        public void SetPacket(Packet p)
        {
            if (AssociatedPacket != null)
            {
                AssociatedPacket.OnExpireWarning -= OnExpireWarning;
            }

            OriginalColor = p.Color;
            // First set the material to the active material -- albeit a muted color one.
            SetMaterial(ActiveMaterial, NeutralMaterial.color);
            IsActive = true;

            // Then animate the color.
            colorChangingCoroutine = StartCoroutine(GraduallyAdjustColor(NeutralMaterial.color, p.Color));
            
            AssociatedPacket = p;
            p.OnExpireWarning += OnExpireWarning;
        }

        public void AdjustRadius(float radius, float initialExtraRadius, bool disappearAfter = false)
        {
            if (resizingCoroutine != null)
            {
                StopCoroutine(resizingCoroutine);
            }
            resizingCoroutine = StartCoroutine(GraduallyAdjustRadius(radius + initialExtraRadius, radius, disappearAfter));
        }

        public void OnExpireWarning(Packet p)
        {
            StartFlashing(p.Color);
        }

        private IEnumerator GraduallyAdjustRadius(
            float fromRadius, float toRadius, bool disappearAfter = false)
        {
            float radius = fromRadius;
            float increment = -0.0005f;

            while (radius > toRadius)
            {
                radius += increment;

                SetRadius(radius);

                yield return null;
            }

            if (disappearAfter)
            {
                Destroy(this.gameObject);
            }
        }

        private IEnumerator GraduallyAdjustColor(Color from, Color to)
        {
            Material mat = IsActive ? ActiveMaterial : NeutralMaterial;
            
            float startTime = Time.fixedTime;
            float t = startTime;
            while (t - startTime <= RingColorChangeDuration)
            {
                Color c = Color.Lerp(from, to, t / RingColorChangeDuration);
                mat.color = c;
                mat.SetColor("_EmissionColor", c);

                yield return null;
            }

            mat.color = to;
            mat.SetColor("_EmissionColor", to);
        }

        private IEnumerator Flash(Color from, Color to)
        {
            while (true)
            {
                Color lerpedColor = Color.Lerp(from, to, Mathf.PingPong(Time.fixedTime * FlashRate, 1));
                ActiveMaterial.color = lerpedColor;
                ActiveMaterial.SetColor("_EmissionColor", lerpedColor);

                yield return null;
            }
        }

        public void StartFlashing(Color c)
        {
            if (colorChangingCoroutine != null)
            {
                StopCoroutine(colorChangingCoroutine);
            }

            colorChangingCoroutine = StartCoroutine(Flash(Color.white, c));
            IsFlashing = true;
        }

        public void EndFlashing()
        {
            if (IsFlashing)
            {
                StopCoroutine(colorChangingCoroutine);

                ActiveMaterial.color = OriginalColor;
                ActiveMaterial.SetColor("_EmissionColor", OriginalColor);

                IsFlashing = false;
            }
        }

        public void SetRadius(float radius)
        {
            Radius = radius;

            var rRoot3Over4 = radius * Mathf.Sqrt(3) / 4.0f;
            var threeFourthsR = 3.0f * radius / 4.0f;
            var rRoot3Over2 = radius * Mathf.Sqrt(3) / 2.0f;
            var halfRingThickness = RingThickness / 2.0f;

            RightUpper.transform.localPosition = Vector3.zero;
            RightUpper.transform.rotation = Quaternion.identity;
            RightUpper.transform.localPosition += new Vector3(halfRingThickness, 0);
            RightUpper.transform.RotateAround(transform.parent.position, Vector3.forward, -30.0f);
            RightUpper.transform.localPosition += new Vector3(-threeFourthsR, rRoot3Over4, 0.0f);

            RightLower.transform.localPosition = Vector3.zero;
            RightLower.transform.rotation = Quaternion.identity;
            RightLower.transform.localPosition += new Vector3(halfRingThickness, 0);
            RightLower.transform.RotateAround(transform.parent.position, Vector3.forward, 30.0f);
            RightLower.transform.localPosition += new Vector3(-threeFourthsR, -rRoot3Over4, 0.0f);

            LeftUpper.transform.localPosition = Vector3.zero;
            LeftUpper.transform.rotation = Quaternion.identity;
            LeftUpper.transform.localPosition += new Vector3(-halfRingThickness, 0);
            LeftUpper.transform.RotateAround(transform.parent.position, Vector3.forward, 30.0f);
            LeftUpper.transform.localPosition += new Vector3(threeFourthsR, rRoot3Over4, 0.0f);

            LeftLower.transform.localPosition = Vector3.zero;
            LeftLower.transform.rotation = Quaternion.identity;
            LeftLower.transform.localPosition += new Vector3(-halfRingThickness, 0);
            LeftLower.transform.RotateAround(transform.parent.position, Vector3.forward, -30.0f);
            LeftLower.transform.localPosition += new Vector3(threeFourthsR, -rRoot3Over4, 0.0f);

            Top.transform.localPosition = Vector3.zero;
            Top.transform.rotation = Quaternion.identity;
            Top.transform.localPosition += new Vector3(-halfRingThickness, 0);
            Top.transform.RotateAround(transform.parent.position, Vector3.forward, 90.0f);
            Top.transform.localPosition += new Vector3(0, rRoot3Over2, 0);

            Bottom.transform.localPosition = Vector3.zero;
            Bottom.transform.rotation = Quaternion.identity;
            Bottom.transform.localPosition += new Vector3(halfRingThickness, 0);
            Bottom.transform.RotateAround(transform.parent.position, Vector3.forward, 90.0f);
            Bottom.transform.localPosition += new Vector3(0, -rRoot3Over2, 0);

            foreach (Transform t in this.transform)
            {
                t.localScale = new Vector3(RingThickness, radius, RingThickness);
            }
        }

        public void SetMaterial(Material m, Color c)
        {
            m.color = c;
            m.SetColor("_EmissionColor", c);

            foreach (Transform t in this.transform)
            {
                t.GetComponent<Renderer>().material = m;
            }
        }

        public void SetMaterial(Material m)
        {
            SetMaterial(m, NeutralMaterial.color);
        }   
    }
}

