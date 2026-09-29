using System.Threading;
using ArmorTheVehicle.Core;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ArmorTheVehicle.UI
{
    public class GameUi : MonoBehaviour
    {
        [SerializeField] private UiPanel _startHint;
        [SerializeField] private UiPanel _resultOverlay;
        [SerializeField] private TMP_Text _resultTitle;

        [SerializeField] private string _winText = "You win";
        [SerializeField] private string _loseText = "You lose";

        [SerializeField] private Color _winColor = new(0.4f, 0.9f, 0.4f);
        [SerializeField] private Color _loseColor = new(0.95f, 0.35f, 0.3f);

        public UniTask ShowStartHintAsync(CancellationToken cancellationToken)
        {
            return _startHint.ShowAsync(cancellationToken);
        }

        public UniTask HideStartHintAsync(CancellationToken cancellationToken)
        {
            return _startHint.HideAsync(cancellationToken);
        }

        public UniTask ShowResultAsync(GameResult result, CancellationToken cancellationToken)
        {
            bool isWin = result == GameResult.Win;

            _resultTitle.text = isWin ? _winText : _loseText;
            _resultTitle.color = isWin ? _winColor : _loseColor;

            return _resultOverlay.ShowAsync(cancellationToken);
        }

        public UniTask HideResultAsync(CancellationToken cancellationToken)
        {
            return _resultOverlay.HideAsync(cancellationToken);
        }
    }
}
