using System;
using UnityEngine;

namespace InternetGame
{
	[Serializable]
	public class PortInfo
	{
		static int next_id;

		public int id;
		public Vector3 location;
		public Quaternion orientation;

		public PortInfo (Vector3 location, Quaternion orientation) {
			this.id = next_id++;
			this.location = location;
			this.orientation = orientation;
		}
	}
}

