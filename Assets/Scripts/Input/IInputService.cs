using System.Threading;
using Cysharp.Threading.Tasks;

namespace ArmorTheVehicle.Input
{
    public interface IInputService
    {
        public bool IsPressed { get; }

        public float HorizontalDelta { get; }

        public UniTask WaitForTapAsync(CancellationToken cancellationToken);
    }
}
