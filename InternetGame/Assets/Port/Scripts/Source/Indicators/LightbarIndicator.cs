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

        public Lightbar Lightbar;

        public bool EnableEmission;

        protected Material NeutralMaterialCopy;
        protected Material ActiveMaterialCopy;

        private Dictionary<Packet, LightChunk> PacketLightChunks;

        void Start()
        {
            //if (!Application.isPlaying && Lightbar == null)
            //{
            //    Lightbar = gameObject.AddComponent<Lightbar>();
            //    Lightbar.Initialize();
            //}
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

            PacketLightChunks = new Dictionary<Packet, LightChunk>();

            Lightbar = GetComponent<Lightbar>();
            if (Lightbar == null)
            {
                Lightbar = gameObject.AddComponent<Lightbar>();
            }
            Lightbar.Initialize();
        }

        public override void OnPacketExpired(object sender, PacketEventArgs p)
        {
            base.OnPacketExpired(sender, p);
        }

        public override void OnPacketEnqueued(object sender, PacketEventArgs p)
        {
            base.OnPacketEnqueued(sender, p);

            PacketLightChunks[p.Packet] = Lightbar.AddLight(
                p.Packet.Color, 
                1 /* light bar width */,
                p.Packet.Patience /* duration of packet drop */);
        }

        public override void OnLinkStarted(object sender, LinkEventArgs l)
        {
            base.OnLinkStarted(sender, l);
        }

        public LightChunk GetLightChunkFor(Packet p)
        {
            return PacketLightChunks[p];
        }
    }
}
