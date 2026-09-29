using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArmorTheVehicle.Input
{
    public class InputService : IInputService, IDisposable
    {
        private readonly InputAction _pressAction = new("Press", InputActionType.Button, "<Pointer>/press");
        private readonly InputAction _deltaAction = new("Delta", InputActionType.Value, "<Pointer>/delta");

        public InputService()
        {
            _pressAction.Enable();
            _deltaAction.Enable();
        }

        public bool IsPressed => _pressAction.IsPressed();

        public float HorizontalDelta => IsPressed ? _deltaAction.ReadValue<Vector2>().x / Screen.width : 0f;

        public UniTask WaitForTapAsync(CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(_pressAction.WasPressedThisFrame,
                cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            _pressAction.Dispose();
            _deltaAction.Dispose();
        }
    }
}
