using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace InternetGame {

	public class PortLoader : MonoBehaviour {

		[NonSerialized]
		public PacketSource[] SourceObjects;
		[NonSerialized]
		public PacketSink[] SinkObjects;

		public List<PortInfo> sources;
		public List<PortInfo> sinks;

		// Use this for initialization
		void Start () {
			InitSrcList ();
			InitSinkList ();
			sources = new List<PortInfo>();
			sinks = new List<PortInfo>();
			SavePorts ();
		}

		void InitSrcList() {
			SourceObjects = FindObjectsOfType (typeof(PacketSource)) as PacketSource[];
		}

		void InitSinkList() {
			SinkObjects = FindObjectsOfType<PacketSink> ();
		}

		void SavePorts() {
			sources.Clear ();
			foreach (PacketSource src in SourceObjects) {
				sources.Add (src.info);
			}
			sinks.Clear ();
//			foreach (PacketSink sink in SinkObjects) {
//				sinks.Add (sink.info);
//			}

			string json = JsonUtility.ToJson (this, true);
			Debug.Log (json);
		}
	}
}