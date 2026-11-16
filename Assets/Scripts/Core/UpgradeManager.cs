using System;
using R3;

namespace Asteroids.Core
{
    public class UpgradeManager : IDisposable
    {
        public ReactiveProperty<float> Dust { get; } = new(0f);

        public void AddDust(float amount)
        {
            if (amount <= 0f) return;
            Dust.Value += amount;
        }

        public bool SpendDust(float amount)
        {
            if (amount <= 0f || Dust.Value < amount) return false;
            Dust.Value -= amount;
            return true;
        }

        public void Dispose() => Dust.Dispose();
    }
}
