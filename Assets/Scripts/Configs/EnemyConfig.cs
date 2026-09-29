using UnityEngine;

namespace ArmorTheVehicle.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Armor The Vehicle/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [SerializeField] private int _maxHealth = 30;
        [SerializeField] private float _detectionRadius = 20f;
        [SerializeField] private float _runSpeed = 9f;
        [SerializeField] private float _turnSpeed = 720f;
        [SerializeField] private float _attackRange = 2.5f;
        [SerializeField] private int _attackDamage = 10;
        [SerializeField] private float _knockbackSpeed = 4f;
        [SerializeField] private float _knockbackDamping = 25f;
        [SerializeField] private float _deathDuration = 1f;

        public int MaxHealth => _maxHealth;
        public float DetectionRadius => _detectionRadius;
        public float RunSpeed => _runSpeed;
        public float TurnSpeed => _turnSpeed;
        public float AttackRange => _attackRange;
        public int AttackDamage => _attackDamage;
        public float KnockbackSpeed => _knockbackSpeed;
        public float KnockbackDamping => _knockbackDamping;
        public float DeathDuration => _deathDuration;
    }
}
