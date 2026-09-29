using System;

namespace ArmorTheVehicle.Gameplay.Enemies
{
    public class KillCounter
    {
        public event Action Changed;

        public int Count { get; private set; }

        public void Add()
        {
            Count++;

            Changed?.Invoke();
        }

        public void Reset()
        {
            Count = 0;

            Changed?.Invoke();
        }
    }
}
