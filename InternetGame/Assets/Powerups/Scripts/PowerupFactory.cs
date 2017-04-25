using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PowerupFactory : MonoBehaviour
    {
        static GameObject InvincibilityPowerupPrefab;
        static GameObject TimeFreezePowerupPrefab;

        public static void LoadResources()
        {
            InvincibilityPowerupPrefab = Resources.Load<GameObject>("Prefabs/CD");
            TimeFreezePowerupPrefab = Resources.Load<GameObject>("Prefabs/CD");

            SplittingLink.LoadResources();
        }

        public static void Initialize()
        {
            LoadResources();
        }

        public static Powerup CreatePowerup(PowerupType powerupType, Vector3 position)
        {
            GameObject container = null; 
            Powerup powerup = null;
            switch (powerupType)
            {
                case PowerupType.Invincibility:
                    container = Instantiate<GameObject>(InvincibilityPowerupPrefab);
                    powerup = container.AddComponent<InvincibilityPowerup>();
                    powerup.transform.position = position;
                    break;
                case PowerupType.TimeFreeze:
                    container = Instantiate<GameObject>(TimeFreezePowerupPrefab);
                    powerup = container.AddComponent<TimeFreezePowerup>();
                    powerup.transform.position = position;
                    break;
                default:
                    Debug.LogError("Trying to instantiate a generic Powerup.");
                    break;
            }

            if (powerup)
            {
                powerup.Initialize();
            }
            return powerup;
        }
    }
}

