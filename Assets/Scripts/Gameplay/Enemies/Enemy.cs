using System;
using ArmorTheVehicle.Configs;
using ArmorTheVehicle.Gameplay.Car;
using ArmorTheVehicle.Gameplay.Combat;
using UnityEngine;
using Zenject;

namespace ArmorTheVehicle.Gameplay.Enemies
{
    [RequireComponent(typeof(Collider))]
    public class Enemy : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Dead
        }

        [SerializeField] private GameObject _model;
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private EnemyHealthBar _healthBar;
        [SerializeField] private Collider _collider;
        [SerializeField] private HitFlash _hitFlash;
        [SerializeField] private ParticleSystem _hitSparks;

        [SerializeField] private int _hitSparkCount = 8;

        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");

        private EnemyConfig _config;
        private CarController _car;
        private KillCounter _kills;
        private Health _health;
        private Action<Enemy> _release;

        private State _state;

        private float _deathTimer;

        private Vector3 _knockbackVelocity;

        [Inject]
        private void Construct(EnemyConfig config, CarController car, KillCounter kills)
        {
            _config = config;
            _car = car;
            _kills = kills;
            _health = new Health(config.MaxHealth);

            _health.Died += OnKilled;
        }

        public void Initialize(Action<Enemy> release)
        {
            _release = release;
        }

        public void Spawn(Vector3 position)
        {
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(Vector3.back));

            _health.Reset();

            _collider.enabled = true;

            _state = State.Idle;

            _healthBar.Hide();
            _explosion.Clear();
            _hitSparks.Clear();
            _hitFlash.ResetState();

            _knockbackVelocity = Vector3.zero;

            _model.SetActive(true);

            _animator.Rebind();
            _animator.Update(0f);
        }

        public void TakeHit(int damage, Vector3 direction)
        {
            _hitSparks.Emit(_hitSparkCount);
            _health.TakeDamage(damage);

            if (_health.IsDead) return;

            _healthBar.Show(_health.Normalized);
            _hitFlash.Play();

            direction.y = 0f;

            _knockbackVelocity = direction.normalized * _config.KnockbackSpeed;
        }

        private void Update()
        {
            ApplyKnockback();

            switch (_state)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Chase:
                    UpdateChase();
                    break;
                case State.Dead:
                    UpdateDead();
                    break;
            }
        }

        private void UpdateIdle()
        {
            float detectionRadius = _config.DetectionRadius;

            if (ToCar().sqrMagnitude > detectionRadius * detectionRadius) return;

            _state = State.Chase;
            _animator.SetBool(IsRunningHash, true);
        }

        private void UpdateChase()
        {
            if (!_car.IsDriving)
            {
                _animator.SetBool(IsRunningHash, false);

                return;
            }

            Vector3 toCar = ToCar();

            float attackRange = _config.AttackRange;

            if (toCar.sqrMagnitude <= attackRange * attackRange)
            {
                HitCar();

                return;
            }

            MoveTowardsCar();
            Turn(toCar);
        }

        private void HitCar()
        {
            _car.Health.TakeDamage(_config.AttackDamage);

            Die();
        }

        private void UpdateDead()
        {
            _deathTimer -= Time.deltaTime;

            if (_deathTimer <= 0f) _release(this);
        }

        private void OnKilled()
        {
            _kills.Add();

            Die();
        }

        private void Die()
        {
            _state = State.Dead;

            _collider.enabled = false;

            _deathTimer = _config.DeathDuration;

            _healthBar.Hide();
            _model.SetActive(false);
            _explosion.Play();
        }

        private void ApplyKnockback()
        {
            if (_knockbackVelocity == Vector3.zero) return;

            transform.position += _knockbackVelocity * Time.deltaTime;

            _knockbackVelocity = Vector3.MoveTowards(_knockbackVelocity, Vector3.zero,
                _config.KnockbackDamping * Time.deltaTime);
        }

        private Vector3 ToCar()
        {
            Vector3 toCar = _car.transform.position - transform.position;

            toCar.y = 0f;

            return toCar;
        }

        private void MoveTowardsCar()
        {
            Vector3 target = _car.transform.position;

            target.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, target,
                _config.RunSpeed * Time.deltaTime);
        }

        private void Turn(Vector3 direction)
        {
            if (direction.sqrMagnitude < Mathf.Epsilon) return;

            Quaternion target = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, target,
                _config.TurnSpeed * Time.deltaTime);
        }
    }
}
