using System;
using ArmorTheVehicle.Gameplay.Enemies;
using UnityEngine;

namespace ArmorTheVehicle.Gameplay.Combat
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private Rigidbody _rigidbody;

        private Action<Bullet> _release;

        private int _damage;
        private float _lifetimeLeft;
        private bool _isFlying;

        public void Initialize(Action<Bullet> release)
        {
            _release = release;
        }

        public void Launch(Vector3 position, Quaternion rotation, float speed, int damage,
            float lifetime)
        {
            transform.SetPositionAndRotation(position, rotation);

            _rigidbody.position = position;
            _rigidbody.rotation = rotation;
            _rigidbody.linearVelocity = rotation * Vector3.forward * speed;

            _damage = damage;
            _lifetimeLeft = lifetime;

            _isFlying = true;

            _trail.Clear();
        }

        private void Update()
        {
            _lifetimeLeft -= Time.deltaTime;

            if (_lifetimeLeft <= 0f)
                Despawn();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isFlying) return;

            if (collision.collider.TryGetComponent(out Enemy enemy))
                enemy.TakeHit(_damage, transform.forward);

            Despawn();
        }

        private void Despawn()
        {
            _isFlying = false;
            _release(this);
        }
    }
}
