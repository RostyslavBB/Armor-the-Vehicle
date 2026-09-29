using System.Threading;
using ArmorTheVehicle.Configs;
using ArmorTheVehicle.Gameplay.Combat;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ArmorTheVehicle.Gameplay.Car
{
    public class CarController : MonoBehaviour
    {
        private CarConfig _config;
        private LevelConfig _levelConfig;

        private Vector3 _startPosition;

        private float _targetSpeed;

        public Health Health { get; private set; }
        public float Speed { get; private set; }
        public bool IsDriving => _targetSpeed > 0f;

        [Inject]
        private void Construct(CarConfig config, LevelConfig levelConfig)
        {
            _config = config;
            _levelConfig = levelConfig;

            Health = new Health(config.MaxHealth);
        }

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            float rate = _targetSpeed > Speed ? _config.Acceleration : _config.Deceleration;

            Speed = Mathf.MoveTowards(Speed, _targetSpeed, rate * Time.deltaTime);

            transform.position += Vector3.forward * (Speed * Time.deltaTime);
        }

        public void StartDriving()
        {
            _targetSpeed = _config.MaxSpeed;
        }

        public void Stop()
        {
            _targetSpeed = 0f;
        }

        public void ResetState()
        {
            transform.position = _startPosition;

            Speed = 0f;
            _targetSpeed = 0f;

            Health.Reset();
        }

        public UniTask WaitForFinishAsync(CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(() =>
                transform.position.z >= _levelConfig.LevelLength,
                cancellationToken: cancellationToken);
        }

        public UniTask WaitForDeathAsync(CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(() =>
                Health.IsDead, cancellationToken: cancellationToken);
        }
    }
}
