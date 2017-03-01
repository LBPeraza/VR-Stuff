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
        public float FadeRate = .5f; // Alpha per second
        public GameObject PickUpHitbox;
        public GameObject RegularHitbox;
        public static Material FadeMaterial;
        public Color Color;

        [HideInInspector]
        public PacketSource Source;
        [HideInInspector]
        public Link Link;
        [HideInInspector]
        public Cursor Cursor;

        public bool IsAtSource;

        public static void LoadResources()
        {
            FadeMaterial = new Material(Resources.Load<Material>("Materials/ConnectorFaded"));
            if (FadeMaterial == null)
            {
                Debug.LogError("Could not find connector fade material");
            }
        }

        public void Initialize(PacketSource source)
        {
            IsAtSource = true;
            Source = source;

            // Set hitbox large for picking up easily.
            RegularHitbox.SetActive(false);
            PickUpHitbox.SetActive(true);

            if (!source.IsEmpty())
            {
                SetColor(source.Peek().Color);
            }

        }

        public void SetColor(Color c)
        {
            Color = c;
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
            foreach (Transform model in ConnectorModel.transform)
            {
                model.GetComponent<Renderer>().material = FadeMaterial;
            }
            SetColor(Color);
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

            if (IsAtSource && !Source.IsEmpty())
            {
                VRTK_InteractGrab grab = e.interactingObject.GetComponentInParent<VRTK_InteractGrab>();
                Cursor cursor = grab.controllerAttachPoint.GetComponentInParent<Cursor>();
                Cursor = cursor;

                Link = LinkController.GetInstance().StartLink(cursor, Source, this);
                Link.OnSever += OnLinkSever;

                IsAtSource = false;

                // Prevent collisions with the cursor hitboxes.
                gameObject.layer = LayerMask.NameToLayer("Connector");

                // Make hitbox smaller for plugging into port.
                RegularHitbox.SetActive(true);
                PickUpHitbox.SetActive(false);

                Cursor.OnGrab(Cursor.DefaultCursorEventArgs);
            }
        }

        private void OnLinkSever(Link severed, SeverCause cause, float totalLength)
        {
            SetGrabbable(false);
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);

            if (LinkController.GetInstance().State == LinkControllerState.DrawingLink)
            {
                if (!hoveredOverSnapDropZone)
                {
                    LinkController.GetInstance().EndLink();
                }
                else
                {
                    // Dropped in snap zone, so the link will complete (will be ended by sink).
                    // Trigger haptic pulse in the meantime.
                    LinkController.GetInstance().OnConnectorSnapping(this);
                }

                LinkController.GetInstance().Cursor.OnDrop(Cursor.DefaultCursorEventArgs);
            }
        }

        public void SetGrabbable(bool grabbable)
        {
            this.isGrabbable = grabbable;
        }

        public virtual void OnSnappedToPort(PacketSink sink)
        {
            SetGrabbable(false);
        }
    }
}

