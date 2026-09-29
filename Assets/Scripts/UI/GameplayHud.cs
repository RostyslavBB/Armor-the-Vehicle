using ArmorTheVehicle.Gameplay.Car;
using ArmorTheVehicle.Gameplay.Combat;
using ArmorTheVehicle.Gameplay.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ArmorTheVehicle.UI
{
    public class GameplayHud : MonoBehaviour
    {
        [SerializeField] private Image _healthFill;
        [SerializeField] private Image _healthTrail;
        [SerializeField] private TMP_Text _killsText;

        [SerializeField] private float _trailDelay = 0.4f;
        [SerializeField] private float _trailSpeed = 0.6f;

        private CarController _car;
        private KillCounter _kills;
        private Health _carHealth;

        private float _trailDelayLeft;

        [Inject]
        private void Construct(CarController car, KillCounter kills)
        {
            _car = car;
            _kills = kills;
        }

        private void Start()
        {
            _carHealth = _car.Health;

            _carHealth.Changed += OnHealthChanged;
            _kills.Changed += OnKillsChanged;

            OnHealthChanged();
            OnKillsChanged();
        }

        private void OnDestroy()
        {
            _carHealth.Changed -= OnHealthChanged;
            _kills.Changed -= OnKillsChanged;
        }

        private void Update()
        {
            if (_trailDelayLeft > 0f)
            {
                _trailDelayLeft -= Time.deltaTime;

                return;
            }

            _healthTrail.fillAmount = Mathf.MoveTowards(_healthTrail.fillAmount,
                _healthFill.fillAmount, _trailSpeed * Time.deltaTime);
        }

        private void OnHealthChanged()
        {
            float value = _carHealth.Normalized;

            if (value >= _healthFill.fillAmount) _healthTrail.fillAmount = value;

            else _trailDelayLeft = _trailDelay;

            _healthFill.fillAmount = value;
        }

        private void OnKillsChanged()
        {
            _killsText.SetText("{0}", _kills.Count);
        }
    }
}
