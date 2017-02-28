using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace InternetGame {

	public enum LoadLocation {
		LevelParams,
		LoaderLevel,
		NoLoad
	}

	[Serializable]
	class Ports {
		public List<ClusterInfo> sinks;
		public List<SourceInfo> sources;

		public Ports() {
			sinks = new List<ClusterInfo> ();
			sources = new List<SourceInfo> ();
		}
	}

	public class PortLoader : MonoBehaviour, ResourceLoadable {

		public string levelName = "default_level";
		public bool SaveOnRun = false;
		public LoadLocation LoadFrom = LoadLocation.LevelParams;

		public GameObject SourceHolder;
		public GameObject SinkHolder;

		public PacketSource SourcePrefab;
		public PacketSink SinkPrefab;

		[Header("Cluster Prefabs")]
		public SinkCluster ClusterPrefab;
		public GameObject SinkPortPrefab;
		public GameObject BackingPrefab;

		private PacketSource[] SourceObjects;
		private SinkCluster[] SinkObjects;

		public void Initialize(LevelParameters levelParams) {
            LoadResources();

			if (SaveOnRun)
				SavePorts ();

			if (LoadFrom == LoadLocation.LevelParams)
				LoadPorts (levelParams.LevelName);
			else if (LoadFrom == LoadLocation.LoaderLevel)
				LoadPorts (levelName);
		}

		public void LoadResources() {
			if (SourceHolder == null) {
				SourceHolder = GameObject.Find ("Sources");
			}
			if (SinkHolder == null) {
				SinkHolder = GameObject.Find ("Sinks");
			}

			if (SourcePrefab == null) {
				SourcePrefab = Resources.Load<PacketSource> ("Prefabs/Source");
			}
			if (SinkPrefab == null) {
				SinkPrefab = Resources.Load<PacketSink> ("Prefabs/Sink");
			}
			if (ClusterPrefab == null) {
				ClusterPrefab = Resources.Load<SinkCluster> ("Prefabs/ClusterPrefab");
			}
			if (SinkPortPrefab == null) {
				SinkPortPrefab = Resources.Load<GameObject> ("Prefabs/SinkFromCluster");
			}
			if (BackingPrefab == null) {
				BackingPrefab = Resources.Load<GameObject> ("Prefabs/ClusterBacking");
			}
		}

		void GetSrcList() {
			SourceObjects = SourceHolder.transform.GetComponentsInChildren<PacketSource> ();
		}

		void GetSinkList() {
			SinkObjects = SinkHolder.transform.GetComponentsInChildren<SinkCluster> ();
		}

		public void SavePorts() {
			Debug.Log ("Saving ports into Assets/Levels/PortMaps/" + levelName + ".json");
			Ports toSave = new Ports ();
			GetSrcList ();
			foreach (PacketSource src in SourceObjects) {
				toSave.sources.Add (src.portInfo);
			}
			GetSinkList ();
			foreach (SinkCluster sink in SinkObjects) {
				toSave.sinks.Add (sink.clusterInfo);
			}

			string json = JsonUtility.ToJson (toSave, true);
			using (FileStream fs = new FileStream (
									   "Assets/Levels/PortMaps/" + levelName + ".json",
						 			   FileMode.Create)) {
				using (StreamWriter writer = new StreamWriter (fs)) {
					writer.Write (json);
				}
			}
		}

		void ClearChildren(GameObject holder) {
            List<GameObject> toDelete = new List<GameObject>();

			foreach (Transform child in holder.transform) {
                toDelete.Add(child.gameObject);
			}

            foreach (GameObject obj in toDelete)
            {
                DestroyImmediate (obj);
            }
		}

		void ClearPorts() {
			ClearChildren (SourceHolder);
			ClearChildren (SinkHolder);
		}

		public void LoadPorts(string levelName) {
			Debug.Log ("Loading ports from Assets/Levels/PortMaps/" + levelName + ".json");
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
			foreach (ClusterInfo info in toLoad.sinks) {
				LoadClusterSink (info);
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

		void LoadClusterSink(ClusterInfo info) {
			SinkCluster cluster = Instantiate<SinkCluster> (ClusterPrefab, SinkHolder.transform);
			cluster.transform.localPosition = info.location;
			cluster.transform.localRotation = info.orientation;
			cluster.Address = info.address;

			foreach (BackingInfo backing in info.backings) {
				GameObject back = Instantiate<GameObject> (BackingPrefab, cluster.transform);
				back.transform.localPosition = backing.location;
				back.transform.localRotation = backing.orientation;
				back.transform.localScale = backing.scale;
				cluster.Backings.Add (back);
			}

			foreach (PortInfo port in info.ports) {
				GameObject portObj = Instantiate<GameObject> (SinkPortPrefab, cluster.transform);
				portObj.transform.localPosition = port.location;
				portObj.transform.localRotation = port.orientation;
				cluster.Ports.Add (portObj);
			}
		}
	}
}