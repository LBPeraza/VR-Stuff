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
            PacketSourceFactory.Initialize();

			if (SaveOnRun)
				SavePorts ();

			if (LoadFrom == LoadLocation.LevelParams)
            {
                var portMapName = levelParams.PortMapName != null ? 
                    levelParams.PortMapName : 
                    levelParams.LevelName;
                LoadPorts(portMapName);
            }
            else if (LoadFrom == LoadLocation.LoaderLevel)
            {
                LoadPorts(levelName);
            }
        }

		public void LoadResources() {
			if (SourceHolder == null) {
				SourceHolder = GameObject.Find ("Sources");
			}
			if (SinkHolder == null) {
				SinkHolder = GameObject.Find ("Sinks");
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
            PacketSourceFactory.CreatePacketSource(
                SourceHolder.transform, info.Address, info.Location, info.Orientation);
        }

		void LoadSink(SinkInfo info) {
			PacketSink sink = Instantiate<PacketSink> (SinkPrefab, SinkHolder.transform);
			sink.transform.localPosition = info.Location;
			sink.transform.localRotation = info.Orientation;
			sink.Address = info.Address;
		}

		void LoadClusterSink(ClusterInfo info) {
			SinkCluster cluster = Instantiate<SinkCluster> (ClusterPrefab, SinkHolder.transform);
			cluster.transform.localPosition = info.Location;
			cluster.transform.localRotation = info.Orientation;
			cluster.Address = info.Address;

			foreach (BackingInfo backing in info.backings) {
                GameObject backingContainer = new GameObject("BackingContainer");
                backingContainer.transform.SetParent(cluster.transform);
                backingContainer.transform.localPosition = backing.location;
                backingContainer.transform.localRotation = backing.orientation;
                //backingContainer.transform.localScale = backing.scale;
                GameObject back = Instantiate<GameObject>(BackingPrefab, backingContainer.transform, false);
                cluster.Backings.Add (back);
			}

			foreach (PortInfo port in info.ports) {
				GameObject portObj = Instantiate<GameObject> (SinkPortPrefab, cluster.transform);
				portObj.transform.localPosition = port.Location;
				portObj.transform.localRotation = port.Orientation;
				cluster.Ports.Add (portObj);
			}
		}
	}
}