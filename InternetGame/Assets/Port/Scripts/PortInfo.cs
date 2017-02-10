using System;
using UnityEngine;

namespace InternetGame
{
	[Serializable]
	public class PortInfo
	{
		public Vector3 location;
		public Quaternion orientation;

		public PortInfo (Vector3 location, Quaternion orientation) {
			this.location = location;
			this.orientation = orientation;
		}
	}
}

