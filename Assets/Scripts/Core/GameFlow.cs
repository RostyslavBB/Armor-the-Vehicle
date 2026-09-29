using System;
using System.Threading;
using ArmorTheVehicle.Gameplay.Car;
using ArmorTheVehicle.Gameplay.Combat;
using ArmorTheVehicle.Gameplay.Enemies;
using ArmorTheVehicle.Gameplay.Level;
using ArmorTheVehicle.Gameplay.Turret;
using ArmorTheVehicle.Input;
using ArmorTheVehicle.UI;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using Zenject;

namespace ArmorTheVehicle.Core
{
    public class GameFlow : IInitializable, IDisposable
    {
        private readonly IInputService _input;

        private readonly LevelBuilder _level;
        private readonly CarController _car;
        private readonly TurretController _turret;
        private readonly BulletPool _bullets;
        private readonly EnemySpawner _enemies;
        private readonly KillCounter _kills;
        private readonly GameUi _ui;
        private readonly CinemachineCamera _camera;

        private readonly CancellationTokenSource _lifetimeCancellation = new();

        public GameFlow(IInputService input, LevelBuilder level, CarController car, TurretController turret,
            BulletPool bullets, EnemySpawner enemies, KillCounter kills, GameUi ui, CinemachineCamera camera)
        {
            _input = input;
            _level = level;
            _car = car;
            _turret = turret;
            _bullets = bullets;
            _enemies = enemies;
            _kills = kills;
            _ui = ui;
            _camera = camera;
        }

        public void Initialize()
        {
            _level.Build();

            RunAsync(_lifetimeCancellation.Token).Forget();
        }

        public void Dispose()
        {
            _lifetimeCancellation.Cancel();
            _lifetimeCancellation.Dispose();
        }

        private async UniTaskVoid RunAsync(CancellationToken cancellation)
        {
            ResetLevel();

            while (!cancellation.IsCancellationRequested)
            {
                await _ui.ShowStartHintAsync(cancellation);
                await _input.WaitForTapAsync(cancellation);

                _ui.HideStartHintAsync(cancellation).Forget();

                _car.StartDriving();
                _turret.enabled = true;

                GameResult result = await WaitForResultAsync(cancellation);

                _turret.enabled = false;
                _car.Stop();

                await _ui.ShowResultAsync(result, cancellation);
                await _input.WaitForTapAsync(cancellation);

                ResetLevel();

                await _ui.HideResultAsync(cancellation);
            }
        }

        private async UniTask<GameResult> WaitForResultAsync(CancellationToken cancellation)
        {
            using var roundCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation);

            int winnerIndex = await UniTask.WhenAny(
                _car.WaitForFinishAsync(roundCancellation.Token),
                _car.WaitForDeathAsync(roundCancellation.Token));

            roundCancellation.Cancel();
            return winnerIndex == 0 ? GameResult.Win : GameResult.Lose;
        }

        private void ResetLevel()
        {
            _car.ResetState();
            _turret.ResetState();

            _turret.enabled = false;

            _bullets.ReleaseAll();
            _enemies.DespawnAll();
            _enemies.SpawnAll();
            _kills.Reset();

            _camera.PreviousStateIsValid = false;
        }
    }
}
