using UnityEngine;
using System.Collections;
using System;

namespace TowerGame
{
    namespace Tower
    {
        public class Tower : AttackableEntity
        {
            public float MaxHealth;
            public float CurrentHealth;

            public void Start()
            {
                CurrentHealth = MaxHealth;
            }

            public override bool ReceiveDamage(float damage)
            {
                CurrentHealth -= damage;
                
                if (CurrentHealth < 0)
                {
                    return true;
                }

                return false;
            }
        }
    }
}

