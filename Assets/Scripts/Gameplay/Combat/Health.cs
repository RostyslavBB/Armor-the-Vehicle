using System;
using UnityEngine;

namespace ArmorTheVehicle.Gameplay.Combat
{
    public class Health
    {
        public event Action Changed;
        public event Action Damaged;
        public event Action Died;

        public Health(int max)
        {
            Max = max;
            Current = max;
        }

        public int Max { get; }
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;
        public float Normalized => (float)Current / Max;

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;

            Current = Mathf.Max(Current - amount, 0);

            Changed?.Invoke();
            Damaged?.Invoke();

            if (IsDead) Died?.Invoke();
        }

        public void Reset()
        {
            Current = Max;

            Changed?.Invoke();
        }
    }
}
