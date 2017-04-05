using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class TextSpawnerFactory
    {
        public static float MinVelocity = 0.4f;
        public static float MaxVelocity = 0.6f;
        public static float InitialAngle = 60.0f;

        public static TextSpawner CreateTextSpawner(
            GameObject container,
            Vector3 spawnPoint)
        {
            var textSpawner = container.AddComponent<TextSpawner>();
            textSpawner.MinVelocity = MinVelocity;
            textSpawner.MaxVelocity = MaxVelocity;
            textSpawner.InitialPosition = spawnPoint;

            // Make the initial vector InitialAngle up from forward.
            Vector3 initial = Vector3.up + container.transform.TransformDirection(container.transform.forward) * Mathf.Sin(Mathf.Deg2Rad * (90.0f - InitialAngle));
            initial.Normalize();
            textSpawner.InitialDirection = initial;

            return textSpawner;
        }

    }
}

