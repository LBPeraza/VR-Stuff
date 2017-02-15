using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class ConnectorFactory : MonoBehaviour
    {
        public static Connector CreateDefaultConnector(PacketSource source, Transform parent)
        {
            var connectorPrefab = Resources.Load<GameObject>("Prefabs/ConnectorPrefab");
            var connectorContainer = Instantiate(connectorPrefab, parent, false);

            var connector = connectorContainer.GetComponent<Connector>();
            connector.Initialize(source);

            return connector;
        }
    }
}

