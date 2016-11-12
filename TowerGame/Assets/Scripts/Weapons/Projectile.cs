using UnityEngine;
using System.Collections;

namespace TowerGame
{
    namespace Weapons
    {
        [RequireComponent(typeof(Rigidbody))]
        public class Projectile : MonoBehaviour
        {
            public GameObject SpawnPoint;
            public float Damage;
            public bool FriendlyFireEnabled;
            public AttackableEntity Firer;

            void OnCollisionEnter(Collision collision)
            {
                var attackableEntity = collision.gameObject.GetComponent<TowerGame.AttackableEntity>();
                if (attackableEntity != null && (FriendlyFireEnabled || attackableEntity != Firer))
                {
                    attackableEntity.ReceiveDamage(Damage);
                    ResetProjectile();
                }
            }

            void ResetProjectile()
            {
                transform.position = SpawnPoint.transform.position;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
}

