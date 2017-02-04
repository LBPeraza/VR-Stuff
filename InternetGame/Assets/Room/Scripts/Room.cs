using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Room : MonoBehaviour
    {
        public GlowWithRoom RoomGlow;
        public GameObject RoomAccents;
        public GameObject Walls;

        public float Width;
        public float Length;
        public float Height;

        public Material WallMaterial;

        private float WallWidth = 0.1f;
        private bool Dirty = false;

        public void Initialize(AudioSource backgroundMusic)
        {
            BuildAccents();

            if (RoomGlow != null)
            {
                RoomGlow.Initialize(backgroundMusic);
            }
        }

        private void OnValidate()
        {
            Dirty = true;
        }

        public void BuildAccents()
        {

        }

        public void BuildRoom()
        {
            if (this.transform.FindChild("Walls"))
            {
                Walls = this.transform.FindChild("Walls").gameObject;
            }
            else
            {
                Walls = new GameObject("Walls");
                Walls.transform.parent = this.transform;
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

            Dirty = false;
        }
    }
}

