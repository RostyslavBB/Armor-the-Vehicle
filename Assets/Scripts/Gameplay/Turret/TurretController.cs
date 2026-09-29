using ArmorTheVehicle.Configs;
using ArmorTheVehicle.Gameplay.Combat;
using ArmorTheVehicle.Input;
using UnityEngine;
using Zenject;

namespace ArmorTheVehicle.Gameplay.Turret
{
    public class TurretController : MonoBehaviour
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private Transform _recoilTarget;

        [SerializeField] private float _recoilDistance = 0.12f;
        [SerializeField] private float _recoilRecoverySpeed = 1.5f;

        private IInputService _input;

        private TurretConfig _config;
        private BulletPool _bullets;

        private Vector3 _recoilTargetRestPosition;

        private float _targetAngle;
        private float _currentAngle;
        private float _angleVelocity;
        private float _fireCooldown;
        private float _recoilOffset;

        [Inject]
        private void Construct(IInputService input, TurretConfig config, BulletPool bullets)
        {
            _input = input;
            _config = config;
            _bullets = bullets;
        }

        private void Awake()
        {
            _recoilTargetRestPosition = _recoilTarget.localPosition;
        }

        private void Update()
        {
            Rotate();
            TickFire();
            TickRecoil();
        }

        public void ResetState()
        {
            _targetAngle = 0f;
            _currentAngle = 0f;
            _angleVelocity = 0f;
            _fireCooldown = 0f;
            _recoilOffset = 0f;

            transform.localRotation = Quaternion.identity;

            _recoilTarget.localPosition = _recoilTargetRestPosition;
        }

        private void Rotate()
        {
            _targetAngle = Mathf.Clamp(
                _targetAngle + _input.HorizontalDelta * _config.RotationSensitivity,
                -_config.MaxAngle,
                _config.MaxAngle);

            _currentAngle = Mathf.SmoothDampAngle(_currentAngle, _targetAngle,
                ref _angleVelocity, _config.SmoothTime);

            transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }

        private void TickFire()
        {
            _fireCooldown -= Time.deltaTime;

            if (!_input.IsPressed || _fireCooldown > 0f) return;

            _fireCooldown = _config.FireInterval;

            _bullets.Fire(_muzzle.position, _muzzle.rotation);
            _muzzleFlash.Play();

            _recoilOffset = _recoilDistance;
        }

        private void TickRecoil()
        {
            _recoilOffset = Mathf.MoveTowards(_recoilOffset, 0f, _recoilRecoverySpeed * Time.deltaTime);
            _recoilTarget.localPosition = _recoilTargetRestPosition + Vector3.back * _recoilOffset;
        }
    }
}
