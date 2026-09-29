using System.Collections.Generic;
using ArmorTheVehicle.Configs;
using UnityEngine;

namespace ArmorTheVehicle.Gameplay.Enemies
{
    public class EnemySpawner
    {
        private const int MaxAttemptsPerEnemy = 30;

        private readonly LevelConfig _config;
        private readonly EnemyPool _pool;

        private readonly List<Vector3> _positions = new();

        public EnemySpawner(LevelConfig config, EnemyPool pool)
        {
            _config = config;
            _pool = pool;
        }

        public void SpawnAll()
        {
            _positions.Clear();

            for (int i = 0; i < _config.EnemyCount; i++)
            {
                if (!TryFindPosition(out Vector3 position)) continue;

                _positions.Add(position);
                _pool.Spawn(position);
            }
        }

        public void DespawnAll()
        {
            _pool.ReleaseAll();
        }

        private bool TryFindPosition(out Vector3 position)
        {
            for (int attempt = 0; attempt < MaxAttemptsPerEnemy; attempt++)
            {
                position = new Vector3(
                    Random.Range(-_config.RoadHalfWidth, _config.RoadHalfWidth),
                    0f,
                    Random.Range(_config.MinSpawnDistance, _config.LevelLength));

                if (IsFarFromOthers(position)) return true;
            }

            position = default;

            return false;
        }

        private bool IsFarFromOthers(Vector3 position)
        {
            float minDistance = _config.MinDistanceBetweenEnemies;
            float minDistanceSqr = minDistance * minDistance;

            foreach (Vector3 other in _positions)
            {
                if ((other - position).sqrMagnitude < minDistanceSqr) return false;
            }

            return true;
        }
    }
}
