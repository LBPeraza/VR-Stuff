using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Cursor : MonoBehaviour
    {
        public Transform follow;

        // Update is called once per frame
        void Update()
        {
            if (follow != null)
            {
                this.transform.position = follow.transform.position;
                this.transform.rotation = follow.transform.rotation;
            }
        }
    }
}

