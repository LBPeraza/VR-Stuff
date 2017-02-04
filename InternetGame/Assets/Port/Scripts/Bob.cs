using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class Bob : MonoBehaviour
    {
        public Vector3 OriginalPosition;
        public float Radius;

        private void Start()
        {
            OriginalPosition = this.transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

