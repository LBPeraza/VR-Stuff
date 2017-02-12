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

        public void Initialize(PacketSource source)
        {
            Source = source;
        }

        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);

            Debug.Log("Connector is grabbed!");
            SetHeld();
            
            LinkController.GetInstance().StartLink(Source, LinkPointer);
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);

            if (snappedInSnapDropZone)
            {
                var sink = storedSnapDropZone.transform.parent.GetComponent<PacketSink>();
                LinkController.GetInstance().EndLink(sink);
            }
            else
            {
                LinkController.GetInstance().EndLink();
            }
            Debug.Log("Connector is ungrabbed");
        }

        public void SetHeld()
        {
            Debug.Log("Setting held position");
            ConnectorModel.transform.rotation = HeldPosition.rotation;
        }

        public void Follow(Transform t)
        {
            //transform.parent = t;
        }
    }
}

