using ArmorTheVehicle.Gameplay.Combat;
using Unity.Cinemachine;
using UnityEngine;

namespace ArmorTheVehicle.Gameplay.Car
{
    public class CarDamageFeedback : MonoBehaviour
    {
        [SerializeField] private CarController _car;
        [SerializeField] private CinemachineImpulseSource _impulse;
        [SerializeField] private HitFlash _hitFlash;

        private Health _health;

        private void Start()
        {
            _health = _car.Health;

            _health.Damaged += OnDamaged;
        }

        private void OnDestroy()
        {
            _health.Damaged -= OnDamaged;
        }

        private void OnDamaged()
        {
            _impulse.GenerateImpulse();
            _hitFlash.Play();
        }
    }
}
