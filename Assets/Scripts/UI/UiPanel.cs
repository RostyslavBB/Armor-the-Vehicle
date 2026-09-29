using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArmorTheVehicle.UI
{
    public class UiPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private float _hiddenScale = 0.8f;

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);

            await AnimateAsync(0f, 1f, cancellationToken);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            await AnimateAsync(1f, 0f, cancellationToken);

            gameObject.SetActive(false);
        }

        private async UniTask AnimateAsync(float from, float to, CancellationToken cancellationToken)
        {
            for (float time = 0f; time < _duration; time += Time.unscaledDeltaTime)
            {
                Apply(Mathf.Lerp(from, to, time / _duration));

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            Apply(to);
        }

        private void Apply(float visibility)
        {
            _canvasGroup.alpha = visibility;

            float scale = Mathf.LerpUnclamped(_hiddenScale, 1f, _scaleCurve.Evaluate(visibility));

            _content.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
