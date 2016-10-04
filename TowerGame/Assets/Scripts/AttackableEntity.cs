using System;
using UnityEngine;

namespace TowerGame
{
    public abstract class AttackableEntity : MonoBehaviour
    {
        // Returns whether this was the finishing blow.
        public abstract bool ReceiveDamage(float damage);
    }
}

