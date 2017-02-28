using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class ConnectorFactory : MonoBehaviour
    {
        public static GameObject ConnectorPrefab;

        public static void LoadResources()
        {
            ConnectorPrefab = Resources.Load<GameObject>("Prefabs/ConnectorPrefab");

            Connector.LoadResources();
        }

        public static void Initialize()
        {
            LoadResources();
        }

        public static Connector CreateDefaultConnector(PacketSource source, Transform parent)
        {
            var connectorContainer = Instantiate(ConnectorPrefab, parent, false);

            var connector = connectorContainer.GetComponent<Connector>();
            connector.Initialize(source);

            return connector;
        }
    }
}

