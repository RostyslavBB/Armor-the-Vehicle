using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ArmorTheVehicle.Gameplay.Enemies
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;

        private Transform _camera;

        [Inject]
        private void Construct(CinemachineCamera camera)
        {
            _camera = camera.transform;
        }

        private void LateUpdate()
        {
            transform.rotation = _camera.rotation;
        }

        public void Show(float normalized)
        {
            gameObject.SetActive(true);

            _fill.fillAmount = normalized;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
