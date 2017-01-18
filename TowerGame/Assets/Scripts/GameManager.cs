using UnityEngine;
using System.Collections;

namespace TowerGame
{
    public class GameManager : MonoBehaviour
    {
        public Transform[] EnemySpawns;
        public TowerGame.Tower.Tower Tower;
        public int TotalNumEnemies;
        public float EnemySpawnInterval;
        public TowerGame.Enemy.Enemy Enemy;

        private int TotalEnemiesSpawned = 0;
        private int LastSpawnPointIndex = 0;
        private float LastEnemySpawnTime;

        // Use this for initialization
        void Start()
        {
            TotalEnemiesSpawned = 0;
            LastSpawnPointIndex = 0;
        }

        void Update()
        {
            if (Tower.CurrentHealth <= 0)
            {
                //Debug.Log("Game over.");
            }

            if (TotalEnemiesSpawned < TotalNumEnemies && LastEnemySpawnTime + EnemySpawnInterval < Time.fixedTime)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            int nextSpawnIndex = (LastSpawnPointIndex + 1) % EnemySpawns.Length;
            if (EnemySpawns.Length > 0)
            {
                TowerGame.Enemy.Enemy enemy = (TowerGame.Enemy.Enemy)Instantiate(Enemy,
                    EnemySpawns[nextSpawnIndex].position, Quaternion.identity);
                enemy.SetTarget(Tower);
                LastEnemySpawnTime = Time.fixedTime;

                TotalEnemiesSpawned++;
            }
        }
    }
}

