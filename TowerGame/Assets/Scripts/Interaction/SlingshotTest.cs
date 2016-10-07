using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Interaction
    {
        public class SlingshotTest : MonoBehaviour
        {

            public TowerGame.Slingshot.Slingshot slingshot;
            public GameObject holder;
            public GameObject slinger;

        	// Use this for initialization
        	void Start ()
            {
        	    slingshot.PickUp(holder, slinger, true /* is left hand picking up */);
				slingshot.StartShot ();
        	}
        	
        	// Update is called once per frame
        	void Update ()
            {
				if (Input.anyKeyDown) {
					slingshot.Shoot ();
				}
        	}
        }
    }
}