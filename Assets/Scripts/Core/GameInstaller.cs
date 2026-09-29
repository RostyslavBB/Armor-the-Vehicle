using ArmorTheVehicle.Configs;
using ArmorTheVehicle.Gameplay.Car;
using ArmorTheVehicle.Gameplay.Combat;
using ArmorTheVehicle.Gameplay.Enemies;
using ArmorTheVehicle.Gameplay.Level;
using ArmorTheVehicle.Gameplay.Turret;
using ArmorTheVehicle.Input;
using ArmorTheVehicle.UI;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace ArmorTheVehicle.Core
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private CarConfig _carConfig;
        [SerializeField] private TurretConfig _turretConfig;
        [SerializeField] private EnemyConfig _enemyConfig;

        [Header("Scene")]
        [SerializeField] private LevelBuilder _levelBuilder;
        [SerializeField] private CarController _car;
        [SerializeField] private TurretController _turret;
        [SerializeField] private CinemachineCamera _carCamera;
        [SerializeField] private GameUi _gameUi;

        [Header("Pools")]
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _bulletsRoot;
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private Transform _enemiesRoot;

        public override void InstallBindings()
        {
            Container.BindInstance(_levelConfig);
            Container.BindInstance(_carConfig);
            Container.BindInstance(_turretConfig);
            Container.BindInstance(_enemyConfig);

            Container.BindInstance(_levelBuilder);
            Container.BindInstance(_car);
            Container.BindInstance(_turret);
            Container.BindInstance(_carCamera);
            Container.BindInstance(_gameUi);

            Container.BindInterfacesTo<InputService>().AsSingle();

            Container.Bind<BulletPool>().AsSingle().WithArguments(_bulletPrefab, _bulletsRoot);
            Container.Bind<EnemyPool>().AsSingle().WithArguments(_enemyPrefab, _enemiesRoot);

            Container.Bind<EnemySpawner>().AsSingle();
            Container.Bind<KillCounter>().AsSingle();

            Container.BindInterfacesTo<GameFlow>().AsSingle();
        }
    }
}
