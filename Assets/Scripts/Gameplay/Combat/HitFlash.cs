using UnityEngine;

namespace ArmorTheVehicle.Gameplay.Combat
{
    public class HitFlash : MonoBehaviour
    {
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private Renderer[] _renderers;

        [SerializeField, ColorUsage(false, true)] private Color _color = Color.white;

        [SerializeField] private float _duration = 0.1f;

        private MaterialPropertyBlock _propertyBlock;

        private float _timeLeft;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            enabled = false;
        }

        private void Update()
        {
            _timeLeft -= Time.deltaTime;

            Apply(_color * Mathf.Clamp01(_timeLeft / _duration));

            if (_timeLeft <= 0f) enabled = false;
        }

        public void Play()
        {
            _timeLeft = _duration;

            enabled = true;
        }

        public void ResetState()
        {
            _timeLeft = 0f;

            Apply(Color.black);

            enabled = false;
        }

        private void Apply(Color emission)
        {
            _propertyBlock.SetColor(EmissionColorId, emission);

            foreach (Renderer target in _renderers)
                target.SetPropertyBlock(_propertyBlock);
        }
    }
}
