using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame {
	public class VirtualController : MonoBehaviour {

		public float speed = 10.0f;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow))
				transform.Translate (speed * Vector3.left * Time.deltaTime);
			
			if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow))
				transform.Translate (speed * Vector3.right * Time.deltaTime);

			if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow))
				transform.Translate (speed * Vector3.up * Time.deltaTime);
		
			if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow))
				transform.Translate (speed * Vector3.down * Time.deltaTime);
		}
	}
}