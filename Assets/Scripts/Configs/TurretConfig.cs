using UnityEngine;

namespace ArmorTheVehicle.Configs
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "Armor The Vehicle/Turret Config")]
    public class TurretConfig : ScriptableObject
    {
        [SerializeField] private float _maxAngle = 60f;
        [SerializeField] private float _rotationSensitivity = 180f;
        [SerializeField] private float _smoothTime = 0.08f;
        [SerializeField] private float _fireInterval = 0.12f;
        [SerializeField] private float _bulletSpeed = 40f;
        [SerializeField] private int _bulletDamage = 10;
        [SerializeField] private float _bulletLifetime = 2f;

        public float MaxAngle => _maxAngle;
        public float RotationSensitivity => _rotationSensitivity;
        public float SmoothTime => _smoothTime;
        public float FireInterval => _fireInterval;
        public float BulletSpeed => _bulletSpeed;
        public int BulletDamage => _bulletDamage;
        public float BulletLifetime => _bulletLifetime;
    }
}
