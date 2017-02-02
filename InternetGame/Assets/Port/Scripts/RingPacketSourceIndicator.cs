using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class RingPacketSourceIndicator : PacketSourceIndicator
    {
        public Material NeutralMaterial;
        public Material ActiveMaterial;

        public Material NeutralMaterialCopy;
        public Material ActiveMaterialCopy;
        public int NumRings;
        public List<Hexagon> Rings;
        public float InitialRingRadius = 0.25f;
        public float RingRadiusIncrement = 0.05f;
        public float RingThickness = 0.015f;

        public override void Initialize(PacketSourceInfo info)
        {
            if (NeutralMaterial == null)
            {
                NeutralMaterial = Resources.Load<Material>("PacketSourceIndicatorNeutralMaterial");
            }
            if (ActiveMaterial == null)
            {
                ActiveMaterial = Resources.Load<Material>("PacketSourceIndicatorActiveMaterial");
            }

            NeutralMaterialCopy = new Material(NeutralMaterial);
            ActiveMaterialCopy = new Material(ActiveMaterial);

            NumRings = info.Capacity;

            Rings = new List<Hexagon>();
            var ringRadius = InitialRingRadius;
            for (int i = 0; i < NumRings; i ++)
            {
                var ringGameObject = new GameObject("ring");
                ringGameObject.transform.parent = this.transform;
                ringGameObject.transform.localPosition = Vector3.zero;

                var ring = ringGameObject.AddComponent<Hexagon>();

                ring.Initialize(ringRadius, RingThickness);
                Rings.Add(ring);

                ringRadius += RingRadiusIncrement;
            }
        }

        private void ActivateRing(Hexagon ring, Color c)
        {
            var mat = new Material(ActiveMaterial);
            mat.color = c;
            mat.SetColor("_EmissionColor", c);

            ring.SetMaterial(mat);
        }

        private void DeactivateRing(Hexagon ring)
        {
            var mat = new Material(NeutralMaterial);
            ring.SetMaterial(mat);
        }

        public void UpdateRingDisplay(int numActiveRings, List<Packet> queuedPackets)
        {
            for (int i = 0; i < numActiveRings; i ++)
            {
                ActivateRing(Rings[i], queuedPackets[i].Color);
            }

            for (int i = numActiveRings; i < NumRings; i++)
            {
                DeactivateRing(Rings[i]);
            }
        }

        public override void UpdatePacketSourceInfo(PacketSourceInfo info)
        {
            UpdateRingDisplay(info.NumQueuedPackets, info.QueuedPackets);
        }
    }
}
