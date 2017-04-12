using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
	[Serializable]
	public class BackingInfo {
		public Vector3 location;
		public Quaternion orientation;
		public Vector3 scale;

		public BackingInfo (Vector3 l, Quaternion o, Vector3 s) {
			location = l;
			orientation = o;
			scale = s;
		}
	}

	[Serializable]
	public class ClusterInfo : PortInfo
	{
		public List<PortInfo> ports;

		public List<BackingInfo> backings;

		public ClusterInfo (Vector3 location, Quaternion orientation,
			string address, List<PortInfo> ports, List<BackingInfo> backings)
			: base(address, location, orientation)
		{
			this.ports = ports;
			this.backings = backings;
		}
	}

    public class SinkCluster : PacketSink
    {
        [Header("Basic")]
        public List<GameObject> Backings;
        public List<GameObject> Ports;

        [HideInInspector]
        public Color Color;

        private Color Blend(Color a, Color b)
        {
            return new Color(
                (a.r + b.r) / 2,
                (a.g + b.g) / 2,
                (a.b + b.b) / 2,
                (a.a + b.a) / 2);
        }

        public override void Initialize()
        {
            base.Initialize();

            var packetColor = (Color)GameUtils.AddressToColor[this.Address];

			foreach (GameObject backing in Backings) {
				if (backing != null) {
                    if (backing.transform.FindChild("FastForwardButton"))
                    {
                        var fastFowardButton = backing.transform.FindChild("FastForwardButton").GetComponent<VRTK.VRTK_Button>();
                        if (fastFowardButton)
                        {
                            fastFowardButton.ValueChanged += FastFowardButtonChanged; 
                        }
                    }

                    List<Renderer> renderers = new List<Renderer>();
                    if (backing.GetComponent<Renderer>())
                    {
                        renderers.Add(backing.GetComponent<Renderer>());
                    }
                    else if (backing.transform.FindChild("Model"))
                    {
                        renderers = new List<Renderer>(backing.transform.FindChild("Model").GetComponentsInChildren<Renderer>()); 
                    }

                    foreach (Renderer r in renderers)
                    {
                        var originalColor = r.material.color;
                        Color = Blend(packetColor, originalColor);
                        r.material.color = Color;
                    }                
				}
			}

            foreach (GameObject port in Ports)
            {
				port.transform.Find ("Model")
					.transform.Find ("Port")
					.GetComponent<Renderer> ().materials [1].color = packetColor;
            }
        }

        private void FastFowardButtonChanged(object sender, VRTK.Control3DEventArgs e)
        {
            SetFastForward(e.value == 1);
        }

        public ClusterInfo clusterInfo {
			get {
				List<PortInfo> ports = new List<PortInfo> ();
				foreach (GameObject port in Ports) {
					Transform t = port.transform;
					ports.Add (new PortInfo (Address, t.localPosition, t.localRotation));
				}
				List<BackingInfo> backings = new List<BackingInfo> ();
				foreach (GameObject backing in Backings) {
					Transform t = backing.transform;
					backings.Add (new BackingInfo (t.localPosition, t.localRotation, t.localScale));
				}
				return new ClusterInfo (
					transform.position,
					transform.rotation,
					Address,
					ports,
				    backings);
			}
		}
    }
}