﻿using System;
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

        public List<Hexagon> Rings;
        public int NumPackets;
        public int NumInnerPacketsToExclude = 1; 
        public float InitialRingRadius = 0.25f;
        public float RingRadiusIncrement = 0.1f;
        public float RingThickness = 0.015f;
        public bool EnableEmission;

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

            NumPackets = 0;

            Rings = new List<Hexagon>();
            var ringRadius = InitialRingRadius;
        }

        private Hexagon AddRing()
        {
            var ringGameObject = new GameObject("ring");
            ringGameObject.transform.parent = this.transform;
            var ring = ringGameObject.AddComponent<Hexagon>();
            ring.EnableFlashing = (NumInnerPacketsToExclude == 0);
            ring.EnableEmission = EnableEmission;

            return ring;
        }

        public void AddAndActivateNewRing(Packet p)
        {
            // First, create a new hexagon to replace the outer one.
            Hexagon outer = AddRing();

            int numRings = Rings.Count - NumInnerPacketsToExclude + 1;
            float radius = numRings * RingRadiusIncrement + InitialRingRadius;

            outer.Initialize(NeutralMaterialCopy, ActiveMaterial,
                    radius + RingRadiusIncrement, RingThickness);
            outer.AdjustRadius(radius, 3*RingRadiusIncrement);
            outer.SetPacket(p);

            Rings.Add(outer);
        }

        public void ShiftRingsDown()
        {
            int i = 0;
            float radius = InitialRingRadius - RingRadiusIncrement;
            
            // Size each ring as the next lower radius.
            foreach (Hexagon ring in Rings)
            {
                bool disappearAfter = (i == 0);
                ring.AdjustRadius(radius, RingRadiusIncrement, disappearAfter);

                radius += RingRadiusIncrement;

                i++;
            }

            // Remove inner-most ring.
            Rings.RemoveAt(0);
        }

        public override void OnPacketExpired(object sender, PacketEventArgs p)
        {
            base.OnPacketDequeued(sender, p);

            if (NumPackets > NumInnerPacketsToExclude)
            {
                ShiftRingsDown();
            }
            NumPackets--;
        }

        public override void OnPacketEnqueued(object sender, PacketEventArgs p)
        {
            base.OnPacketEnqueued(sender, p);

            NumPackets++;

            if (NumPackets > NumInnerPacketsToExclude)
            {
                AddAndActivateNewRing(p.Packet);
            }
        }

        public override void OnLinkStarted(object sender, LinkEventArgs l)
        {
            base.OnLinkStarted(sender, l);

            if (NumPackets > NumInnerPacketsToExclude)
            {
                ShiftRingsDown();
            }
            NumPackets--;
        }
    }
}
