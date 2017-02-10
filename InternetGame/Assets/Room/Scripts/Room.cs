using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    [ExecuteInEditMode]
    public class Room : MonoBehaviour
    {
        public GlowWithRoom RoomGlow;
        public GameObject RoomAccents;
        public GameObject Walls;

        public float Width;
        public float Length;
        public float Height;

        public float AccentWidth;
        public float AccentDepth;

        public Material WallMaterial;
        public Material AccentMaterial;

        public bool EnableAccents;

        private float WallWidth = 0.1f;
        private bool Dirty = false;

        public void Initialize(AudioSource backgroundMusic)
        {
            BuildRoom();

            if (EnableAccents)
            {
                BuildAccents();
            }

            if (RoomGlow == null)
            {
                RoomGlow = RoomAccents.AddComponent<GlowWithRoom>();
            }
            RoomGlow.Initialize(backgroundMusic, AccentMaterial);
        }

        private void OnValidate()
        {
            Dirty = true;
        }

        public void Update()
        {
            if (!Application.isPlaying && Dirty)
            {
                BuildRoom();

                if (EnableAccents)
                {
                    BuildAccents();
                }

                Dirty = false;
            }
        }

        public void BuildAccents()
        {
            // First, clean existing accents and generate accent cubes.
            if (RoomAccents != null)
            {
                // Destroy existing container and repopulate.
                DestroyImmediate(RoomAccents);
            }
            RoomAccents = new GameObject("Accents");
            RoomAccents.transform.parent = this.transform;

            for (int i = 0; i < 12; i++)
            {
                var accent = GameObject.CreatePrimitive(PrimitiveType.Cube);
                accent.transform.parent = RoomAccents.transform;
            }

            var floorA = RoomAccents.transform.GetChild(0);
            var ceilingA = RoomAccents.transform.GetChild(1);
            Vector3 floorAScale = new Vector3(Width, AccentDepth, AccentWidth);
            floorA.localScale = floorAScale;
            ceilingA.localScale = floorAScale;
            floorA.localPosition = new Vector3(0.0f, 0, 0.0f);
            ceilingA.localPosition = new Vector3(0.0f, Height + WallWidth, 0.0f);

            var floorB = RoomAccents.transform.GetChild(2);
            var ceilingB = RoomAccents.transform.GetChild(3);
            Vector3 floorBScale = new Vector3(AccentWidth, AccentDepth, Length);
            floorB.localScale = floorBScale;
            ceilingB.localScale = floorBScale;
            floorB.localPosition = new Vector3(0.0f, 0, 0.0f);
            ceilingB.localPosition = new Vector3(0.0f, Height + WallWidth, 0.0f);

            var shortWall1A = RoomAccents.transform.GetChild(4);
            var shortWall2A = RoomAccents.transform.GetChild(5);
            Vector3 shortWall1AScale = new Vector3(AccentDepth, Height + 2 * WallWidth, AccentWidth);
            shortWall1A.localScale = shortWall1AScale;
            shortWall2A.localScale = shortWall1AScale;
            shortWall1A.localPosition = new Vector3(Width / 2.0f, Height / 2.0f, 0.0f);
            shortWall2A.localPosition = new Vector3(-Width / 2.0f, Height / 2.0f, 0.0f);

            var shortWall1B = RoomAccents.transform.GetChild(6);
            var shortWall2B = RoomAccents.transform.GetChild(7);
            Vector3 shortWall1BScale = new Vector3(AccentDepth, AccentWidth, Length);
            shortWall1B.localScale = shortWall1BScale;
            shortWall2B.localScale = shortWall1BScale;
            shortWall1B.localPosition = new Vector3(Width / 2.0f, Height / 2.0f, 0.0f);
            shortWall2B.localPosition = new Vector3(-Width / 2.0f, Height / 2.0f, 0.0f);

            var longWall1A = RoomAccents.transform.GetChild(8);
            var longWall2A = RoomAccents.transform.GetChild(9);
            Vector3 longWall1AScale = new Vector3(Width, AccentWidth, AccentDepth);
            longWall1A.localScale = longWall1AScale;
            longWall2A.localScale = longWall1AScale;
            longWall1A.localPosition = new Vector3(0, Height / 2.0f, -Length / 2.0f);
            longWall2A.localPosition = new Vector3(0, Height / 2.0f, Length / 2.0f);

            var longWall1B = RoomAccents.transform.GetChild(10);
            var longWall2B = RoomAccents.transform.GetChild(11);
            Vector3 longWall1BScale = new Vector3(AccentWidth, Height + 2 * WallWidth, AccentDepth);
            longWall1B.localScale = longWall1BScale;
            longWall2B.localScale = longWall1BScale;
            longWall1B.localPosition = new Vector3(0, Height / 2.0f, -Length / 2.0f);
            longWall2B.localPosition = new Vector3(0, Height / 2.0f, Length / 2.0f);

            // Set all of the walls to the appropriate material.
            for (int i = 0; i < RoomAccents.transform.childCount; i++)
            {
                var accent = RoomAccents.transform.GetChild(i)
                    .gameObject.GetComponent<MeshRenderer>().material = AccentMaterial;
            }
        }

        public void BuildRoom()
        {
            // First, clean existing walls and generate wall cubes.
            if (Walls != null)
            {
                // Destroy existing container and repopulate.
                DestroyImmediate(Walls);
            }
            Walls = new GameObject("Walls");
            Walls.transform.parent = this.transform;

            for (int i = 0; i < 6; i++)
            {
                var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.transform.parent = Walls.transform;
            }

            var floor = Walls.transform.GetChild(0);
            Vector3 floorScale = new Vector3(Width + 2 * WallWidth, WallWidth, Length + 2 * WallWidth);
            floor.localScale = floorScale;
            floor.localPosition = new Vector3(0.0f, -WallWidth / 2, 0.0f);

            var ceiling = Walls.transform.GetChild(1);
            ceiling.localScale = floorScale;
            ceiling.localPosition = new Vector3(0.0f, Height + WallWidth + WallWidth / 2.0f, 0.0f);

            var shortWallA = Walls.transform.GetChild(2);
            Vector3 shortWallScale = new Vector3(Width + 2 * WallWidth, Height + 2 * WallWidth, WallWidth);
            shortWallA.localScale = shortWallScale;
            shortWallA.localPosition = new Vector3(0.0f, (Height + 2 * WallWidth) / 2.0f, Length / 2.0f + WallWidth / 2.0f);

            var shortWallB = Walls.transform.GetChild(3);
            shortWallB.localScale = shortWallScale;
            shortWallB.localPosition = new Vector3(0.0f, (Height + 2 * WallWidth) / 2.0f, -(Length / 2.0f + WallWidth / 2.0f));

            var longWallA = Walls.transform.GetChild(4);
            Vector3 longWallScale = new Vector3(WallWidth, Height + 2 * WallWidth, Length + 2 * WallWidth);
            longWallA.localScale = longWallScale;
            longWallA.localPosition = new Vector3(-(Width / 2.0f + WallWidth / 2.0f), Height / 2.0f + WallWidth, 0.0f);

            var longWallB = Walls.transform.GetChild(5);
            longWallB.localScale = longWallScale;
            longWallB.localPosition = new Vector3((Width / 2.0f + WallWidth / 2.0f), Height / 2.0f + WallWidth, 0.0f);

            // Set all of the walls to the appropriate material.
            for (int i = 0; i < Walls.transform.childCount; i++)
            {
                var wall = Walls.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = WallMaterial;
            }
        }
    }
}

