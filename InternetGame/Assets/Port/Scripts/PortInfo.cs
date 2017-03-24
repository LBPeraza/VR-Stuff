using System;
using UnityEngine;

namespace InternetGame
{
	[Serializable]
	public class PortInfo
	{
		public Vector3 Location;
		public Quaternion Orientation;
        public string Address;

		public PortInfo (string address, Vector3 location, Quaternion orientation) {
			this.Location = location;
			this.Orientation = orientation;
            this.Address = address;
		}
	}
}

