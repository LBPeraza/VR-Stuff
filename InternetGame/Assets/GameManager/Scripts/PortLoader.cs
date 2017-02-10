using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace InternetGame {

	[Serializable]
	class Ports {
		public List<SinkInfo> sinks;
		public List<SourceInfo> sources;

		public Ports() {
			sinks = new List<SinkInfo> ();
			sources = new List<SourceInfo> ();
		}
	}

	public class PortLoader : MonoBehaviour {

		public string levelName = "default_level";
		public bool SaveOnRun = false;

		public PacketSource[] SourceObjects;
		public PacketSink[] SinkObjects;

		public GameObject SourceHolder;
		public GameObject SinkHolder;

		public PacketSource SourcePrefab;
		public PacketSink SinkPrefab;

		public void Initialize(LevelParameters levelParams) {
			if (SourceHolder == null) {
				SourceHolder = GameObject.Find ("Sources");
			}
			if (SinkHolder == null) {
				SinkHolder = GameObject.Find ("Sinks");
			}

			if (SourcePrefab == null) {
				SourcePrefab = Resources.Load<PacketSource> ("Source");
			}
			if (SinkPrefab == null) {
				SinkPrefab = Resources.Load<PacketSink> ("Sink");
			}

			if (SaveOnRun)
				SavePorts ();

			LoadPorts (levelParams.LevelName);
		}

		void GetSrcList() {
			SourceObjects = SourceHolder.transform.GetComponentsInChildren<PacketSource> ();
		}

		void GetSinkList() {
			SinkObjects = SinkHolder.transform.GetComponentsInChildren<PacketSink> ();
		}

		public void SavePorts() {
			Debug.Log ("Saving ports into Assets/Levels/PortMaps/" + levelName + ".json");
			Ports toSave = new Ports ();
			GetSrcList ();
			foreach (PacketSource src in SourceObjects) {
				toSave.sources.Add (src.portInfo);
			}
			GetSinkList ();
			foreach (PacketSink sink in SinkObjects) {
				toSave.sinks.Add (sink.portInfo);
			}

			string json = JsonUtility.ToJson (toSave);
			using (FileStream fs = new FileStream (
									   "Assets/Levels/PortMaps/" + levelName + ".json",
						 			   FileMode.Create)) {
				using (StreamWriter writer = new StreamWriter (fs)) {
					writer.Write (json);
				}
			}
		}

		void ClearChildren(GameObject holder) {
			foreach (Transform child in holder.transform) {
				GameObject.Destroy (child.gameObject);
			}
		}

		void ClearPorts() {
			ClearChildren (SourceHolder);
			ClearChildren (SinkHolder);
		}

		public void LoadPorts(string levelName) {
			ClearPorts ();
			string json;
			using (FileStream fs = new FileStream (
				                       "Assets/Levels/PortMaps/" + levelName + ".json",
				                       FileMode.Open)) {
				using (StreamReader reader = new StreamReader (fs)) {
					json = reader.ReadToEnd ();
				}
			}
			Ports toLoad = JsonUtility.FromJson<Ports> (json);
			foreach (SourceInfo info in toLoad.sources) {
				LoadSource (info);
			}
			foreach (SinkInfo info in toLoad.sinks) {
				LoadSink (info);
			}
		}

		void LoadSource(SourceInfo info) {
			PacketSource src = Instantiate<PacketSource> (SourcePrefab, SourceHolder.transform);
			src.transform.localPosition = info.location;
			src.transform.localRotation = info.orientation;
		}

		void LoadSink(SinkInfo info) {
			PacketSink sink = Instantiate<PacketSink> (SinkPrefab, SinkHolder.transform);
			sink.transform.localPosition = info.location;
			sink.transform.localRotation = info.orientation;
			sink.Address = info.address;
		}
	}
}