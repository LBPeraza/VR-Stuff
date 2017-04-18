﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InternetGame
{
    public class PowerupFactory : MonoBehaviour
    {
        static InvincibilityPowerup InvincibilityPowerupPrefab;
        static TimeFreezePowerup TimeFreezePowerupPrefab;

        public static void LoadResources()
        {
            InvincibilityPowerupPrefab = Resources.Load<InvincibilityPowerup>("Prefabs/InvincibilityPowerup");
            TimeFreezePowerupPrefab = Resources.Load<TimeFreezePowerup>("Prefabs/TimeFreezePowerup");

            SplittingLink.LoadResources();
        }

        public static void Initialize()
        {
            LoadResources();
        }

        public static Powerup CreatePowerup(PowerupType powerupType, Vector3 position)
        {
            Powerup powerup;
            switch (powerupType)
            {
                case PowerupType.Invincibility:
                    powerup = Instantiate<InvincibilityPowerup>(InvincibilityPowerupPrefab);
                    powerup.transform.position = position;
                    return powerup;
                case PowerupType.TimeFreeze:
                    powerup = Instantiate<TimeFreezePowerup>(TimeFreezePowerupPrefab);
                    powerup.transform.position = position;
                    return powerup;
                default:
                    Debug.LogError("Trying to instantiate a generic Powerup.");
                    return null;
            }
        }
    }
}

