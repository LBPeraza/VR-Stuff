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

        public override void Initialize(PacketSource source)
        {
            base.Initialize(source);

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

            NumRings = Source.Capacity;

            Rings = new List<Hexagon>();
            var ringRadius = InitialRingRadius;

            //// Make NumRings Hexagons.
            //for (int i = 0; i < NumRings; i ++)
            //{
            //    var ring = AddRing();

            //    ring.Initialize(ringRadius, RingThickness);
            //    Rings.Add(ring);

            //    ringRadius += RingRadiusIncrement;
            //}
        }

        private Hexagon AddRing()
        {
            var ringGameObject = new GameObject("ring");
            ringGameObject.transform.parent = this.transform;
            ringGameObject.transform.localPosition = Vector3.zero;
            var ring = ringGameObject.AddComponent<Hexagon>();

            return ring;
        }

        public void AddAndActivateNewRing(Packet p)
        {
            // First, create a new hexagon to replace the outer one.
            Hexagon outer = AddRing();

            int numRings = Rings.Count;
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

        public override void OnPacketExpired(Packet p)
        {
            base.OnPacketDequeued(p);

            ShiftRingsDown();
        }

        public override void OnPacketEnqueued(Packet p)
        {
            base.OnPacketEnqueued(p);

            AddAndActivateNewRing(p);
        }

        public override void OnLinkStarted(Link l)
        {
            base.OnLinkStarted(l);

            ShiftRingsDown();
        }
    }
}
