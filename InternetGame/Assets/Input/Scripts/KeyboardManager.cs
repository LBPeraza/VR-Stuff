using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
	public class KeyboardManager : MonoBehaviour
	{
		public ClickedEventHandler LeftMouseClicked;
		public ClickedEventHandler LeftMouseUnclicked;
		public ClickedEventHandler RightMouseClicked;
		public ClickedEventHandler RightMouseUnclicked;

		public static ClickedEventArgs emptyArgs;

		private bool leftMouseDown;
		private bool rightMouseDown;

		void Start() {
			emptyArgs.controllerIndex = 0;
			emptyArgs.flags = 0;
			emptyArgs.padX = 0;
			emptyArgs.padY = 0;
		}

		void Update() {

			if (Input.GetMouseButtonDown (0))
				LeftMouseClicked (this, emptyArgs);
			if (Input.GetMouseButtonUp (0))
				LeftMouseUnclicked (this, emptyArgs);
//			if (Input.GetMouseButton (0)) {
//				Debug.Log ("lmb down");
//				if (!leftMouseDown)
//					LeftMouseClicked (this, emptyArgs);
//				leftMouseDown = true;
//			} else
//				leftMouseDown = false;
//
//			if (Input.GetMouseButton (1)) {
//				if (!rightMouseDown)
//					RightMouseClicked (this, emptyArgs);
//				rightMouseDown = true;
//			} else
//				rightMouseDown = false;
		}
	}
}

