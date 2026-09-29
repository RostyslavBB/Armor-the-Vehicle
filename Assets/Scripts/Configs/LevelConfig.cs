using UnityEngine;

namespace ArmorTheVehicle.Configs
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Armor The Vehicle/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private float _levelLength = 200f;
        [SerializeField] private int _enemyCount = 20;
        [SerializeField] private float _minSpawnDistance = 30f;
        [SerializeField] private float _minDistanceBetweenEnemies = 4f;
        [SerializeField] private float _roadHalfWidth = 4f;

        public float LevelLength => _levelLength;
        public int EnemyCount => _enemyCount;
        public float MinSpawnDistance => _minSpawnDistance;
        public float MinDistanceBetweenEnemies => _minDistanceBetweenEnemies;
        public float RoadHalfWidth => _roadHalfWidth;
    }
}
