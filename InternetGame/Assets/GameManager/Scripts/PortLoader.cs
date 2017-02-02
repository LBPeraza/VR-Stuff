using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace InternetGame {

	[Serializable]
	class Ports {
		public List<PortInfo> sinks;
		public List<PortInfo> sources;

		public Ports() {
			sinks = new List<PortInfo> ();
			sources = new List<PortInfo> ();
		}
	}

	public class PortLoader : MonoBehaviour {

		public PacketSource[] SourceObjects;
		public PacketSink[] SinkObjects;

		public GameObject SourceHolder;
		public GameObject SinkHolder;

		public GameObject SourcePrefab;
		public GameObject SinkPrefab;

		public void Initialize() {
			if (SourceHolder == null) {
				SourceHolder = GameObject.Find ("Sources");
			}
			if (SinkHolder == null) {
				SinkHolder = GameObject.Find ("Sinks");
			}

			SourcePrefab = Resources.Load<GameObject> ("Source");
			SinkPrefab = Resources.Load<GameObject> ("Sink");
		}

		void InitSrcList() {
			SourceObjects = FindObjectsOfType<PacketSource> ();
		}

		void InitSinkList() {
			SinkObjects = FindObjectsOfType<PacketSink> ();
		}

		void SavePorts(string levelName) {
			Ports toSave = new Ports ();
			InitSrcList ();
			foreach (PacketSource src in SourceObjects) {
				toSave.sources.Add (src.info);
			}
			InitSinkList ();
			foreach (PacketSink sink in SinkObjects) {
				toSave.sinks.Add (sink.info);
			}

			string json = JsonUtility.ToJson (toSave, true);
			using (FileStream fs = new FileStream (
									   "Assets/Levels/" + levelName + ".json",
						 			   FileMode.Create)) {
				using (StreamWriter writer = new StreamWriter (fs)) {
					writer.Write (json);
				}
			}
		}

		void LoadPorts(string levelName) {
			string json;
			using (FileStream fs = new FileStream (
				                       "Assets/Levels/" + levelName + ".json",
				                       FileMode.Open)) {
				using (StreamReader reader = new StreamReader (fs)) {
					json = reader.ReadToEnd ();
				}
			}
			Ports toLoad = JsonUtility.FromJson<Ports> (json);
			foreach (PortInfo info in toLoad.sources) {
				LoadSource (info);
			}
			foreach (PortInfo info in toLoad.sinks) {
				LoadSink (info);
			}
		}

		void LoadSource(PortInfo info) {
			GameObject src = Instantiate<GameObject> (SourcePrefab, SourceHolder.transform);
			src.transform.localPosition = info.location;
			src.transform.localRotation = info.orientation;
		}

		void LoadSink(PortInfo info) {
			GameObject sink = Instantiate<GameObject> (SinkPrefab, SinkHolder.transform);
			sink.transform.localPosition = info.location;
			sink.transform.localRotation = info.orientation;
		}
	}
}