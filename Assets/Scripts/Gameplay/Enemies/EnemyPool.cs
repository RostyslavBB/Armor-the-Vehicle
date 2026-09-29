using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace ArmorTheVehicle.Gameplay.Enemies
{
    public class EnemyPool
    {
        private const int DefaultCapacity = 32;
        private const int MaxSize = 128;

        private readonly Enemy _prefab;
        private readonly Transform _root;
        private readonly DiContainer _container;

        private readonly ObjectPool<Enemy> _pool;

        private readonly List<Enemy> _active = new(DefaultCapacity);

        public EnemyPool(Enemy prefab, Transform root, DiContainer container)
        {
            _prefab = prefab;
            _root = root;
            _container = container;
            _pool = new ObjectPool<Enemy>(Create, OnGet, OnRelease, OnDestroy,
                true, DefaultCapacity, MaxSize);
        }

        public void Spawn(Vector3 position)
        {
            _pool.Get().Spawn(position);
        }

        public void ReleaseAll()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                _pool.Release(_active[i]);
        }

        private Enemy Create()
        {
            Enemy enemy = _container.InstantiatePrefabForComponent<Enemy>(_prefab, _root);

            enemy.Initialize(Release);

            return enemy;
        }

        private void Release(Enemy enemy)
        {
            _pool.Release(enemy);
        }

        private void OnGet(Enemy enemy)
        {
            enemy.gameObject.SetActive(true);

            _active.Add(enemy);
        }

        private void OnRelease(Enemy enemy)
        {
            enemy.gameObject.SetActive(false);

            _active.Remove(enemy);
        }

        private static void OnDestroy(Enemy enemy)
        {
            Object.Destroy(enemy.gameObject);
        }
    }
}
