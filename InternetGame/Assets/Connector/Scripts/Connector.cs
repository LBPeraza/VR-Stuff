using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace InternetGame
{
    public class Connector : VRTK.VRTK_InteractableObject
    {
        [Header("Connector Options", order = 7)]

        public Transform HeldPosition;
        public Transform LinkPointer;
        public GameObject ConnectorModel;
        public PacketSource Source;

        public float FadeRate = .5f; // Alpha per second

        public bool IsAtSource;

        public void Initialize(PacketSource source)
        {
            IsAtSource = true;
            Source = source;

            if (!source.IsEmpty())
            {
                SetColor(source.Peek().Color);
            }
        }

        public void SetColor(Color c)
        {
            Transform cover = ConnectorModel.transform.FindChild("Cover");
            if (cover != null)
            {
                cover.GetComponent<Renderer>().material.color = c;
            }
            else
            {
                Debug.LogWarning("Could not set color of connector " +
                    "because could not find 'Cover' object.");
            }
        }

        public virtual void Fade()
        {
            StartCoroutine(GraduallyFade());
        }

        private IEnumerator GraduallyFade()
        {
            float t = 0.0f;
            while (t < (1.0f / FadeRate))
            {
                foreach (Transform model in ConnectorModel.transform)
                {
                    Material mat = model.GetComponent<Renderer>().material;
                    Color originalColor = mat.color;
                    Color nextColor = new Color(
                        originalColor.r,
                        originalColor.g,
                        originalColor.b,
                        originalColor.a - (FadeRate * Time.deltaTime));

                    model.GetComponent<Renderer>().material.color = nextColor;
                }

                t += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }

        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);

            if (IsAtSource)
            {
                LinkController.GetInstance().StartLink(Source, this);

                IsAtSource = false;
            }
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);

            if (LinkController.GetInstance().State == LinkControllerState.DrawingLink 
                    && !hoveredOverSnapDropZone)
            {
                // End link in the air.
                LinkController.GetInstance().EndLink();
            }
        }

        public virtual void OnSnappedToPort(PacketSink sink)
        {
            // Don't let the user pick the connector back up.
            this.isGrabbable = false;
        }
    }
}

