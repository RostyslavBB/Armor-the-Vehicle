using UnityEngine;

namespace ArmorTheVehicle.Configs
{
    [CreateAssetMenu(fileName = "CarConfig", menuName = "Armor The Vehicle/Car Config")]
    public class CarConfig : ScriptableObject
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _maxSpeed = 8f;
        [SerializeField] private float _acceleration = 4f;
        [SerializeField] private float _deceleration = 6f;

        public int MaxHealth => _maxHealth;
        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
    }
}
