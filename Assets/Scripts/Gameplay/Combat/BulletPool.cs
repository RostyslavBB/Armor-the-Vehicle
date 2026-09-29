using System.Collections.Generic;
using ArmorTheVehicle.Configs;
using UnityEngine;
using UnityEngine.Pool;

namespace ArmorTheVehicle.Gameplay.Combat
{
    public class BulletPool
    {
        private const int DefaultCapacity = 32;
        private const int MaxSize = 128;

        private readonly Bullet _prefab;
        private readonly Transform _root;
        private readonly TurretConfig _config;

        private readonly ObjectPool<Bullet> _pool;

        private readonly List<Bullet> _active = new(DefaultCapacity);

        public BulletPool(Bullet prefab, Transform root, TurretConfig config)
        {
            _prefab = prefab;
            _root = root;
            _config = config;
            _pool = new ObjectPool<Bullet>(Create, OnGet, OnRelease, OnDestroy,
                true, DefaultCapacity, MaxSize);
        }

        public void Fire(Vector3 position, Quaternion rotation)
        {
            Bullet bullet = _pool.Get();

            bullet.Launch(position, rotation, _config.BulletSpeed, _config.BulletDamage,
                _config.BulletLifetime);
        }

        public void ReleaseAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                _pool.Release(_active[i]);
        }

        private Bullet Create()
        {
            Bullet bullet = Object.Instantiate(_prefab, _root);

            bullet.Initialize(Release);

            return bullet;
        }

        private void Release(Bullet bullet)
        {
            _pool.Release(bullet);
        }

        private void OnGet(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);

            _active.Add(bullet);
        }

        private void OnRelease(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);

            _active.Remove(bullet);
        }

        private static void OnDestroy(Bullet bullet)
        {
            Object.Destroy(bullet.gameObject);
        }
    }
}
