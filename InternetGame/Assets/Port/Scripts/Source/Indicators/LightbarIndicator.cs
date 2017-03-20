using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    [ExecuteInEditMode]
    public class LightbarIndicator : PacketSourceIndicator
    {
        public Material NeutralMaterial;
        public Material ActiveMaterial;

        public bool EnableEmission;

        protected Material NeutralMaterialCopy;
        protected Material ActiveMaterialCopy;

        protected Lightbar Lightbar;

        // Use this for initialization
        void Start()
        {
            if (!Application.isPlaying && Lightbar == null)
            {
                Lightbar = gameObject.AddComponent<Lightbar>();
                Lightbar.Initialize();
            }
        }

        public override void LoadResources()
        {
            base.LoadResources();

            if (NeutralMaterial == null)
            {
                NeutralMaterial = Resources.Load<Material>("Materials/PacketSourceIndicatorNeutralMaterial");
            }
            if (ActiveMaterial == null)
            {
                ActiveMaterial = Resources.Load<Material>("Materials/PacketSourceIndicatorActiveMaterial");
            }
        }

        public override void Initialize(PacketProcessor processor)
        {
            base.Initialize(processor);

            NeutralMaterialCopy = new Material(NeutralMaterial);
            ActiveMaterialCopy = new Material(ActiveMaterial);

            Lightbar = GetComponent<Lightbar>();
            if (Lightbar == null)
            {
                Lightbar = gameObject.AddComponent<Lightbar>();
            }
            Lightbar.Initialize();
        }

        public override void OnPacketExpired(object sender, PacketEventArgs p)
        {
            base.OnPacketDequeued(sender, p);
        }

        public override void OnPacketEnqueued(object sender, PacketEventArgs p)
        {
            base.OnPacketEnqueued(sender, p);

            //if (Source.QueuedPackets.Count == 1 && Source.Peek() == p)
            //{
            //    Debug.Log("fast light bar");
            //    Lightbar.AddLight(p.Color, 1, 1.5f);
            //}
            //else
            //{
            //    Debug.Log("slow light bar");
            //    Lightbar.AddLight(p.Color, 1, p.Patience);
            //}
        }

        public override void OnLinkStarted(object sender, LinkEventArgs l)
        {
            base.OnLinkStarted(sender, l);

        }

    }
}
