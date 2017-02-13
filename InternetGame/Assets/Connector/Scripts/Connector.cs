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
        }

        public virtual void Fade()
        {
            Debug.Log("Fading connector");
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
            Debug.Log("Connector is snapped to port");
            this.isGrabbable = false;
        }

        public void Follow(Transform t)
        {
            //transform.parent = t;
        }
    }
}

