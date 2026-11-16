using System;
using R3;

namespace Asteroids.Core
{
    public class GrowthController : IDisposable
    {
        // Mass required to enter each stage (index 0 = stage 1, index 4 = stage 5)
        public static readonly float[] StageThresholds = { 0f, 10f, 50f, 200f, 1000f };
        public const int MaxStage = 5;

        public ReactiveProperty<float> Mass { get; } = new(1f);
        public ReactiveProperty<int> CurrentStage { get; } = new(1);

        public void AddMass(float amount)
        {
            if (amount <= 0f) return;
            Mass.Value += amount;
            RefreshStage();
        }

        public void SetMass(float mass)
        {
            Mass.Value = Math.Max(0f, mass);
            RefreshStage();
        }

        private void RefreshStage()
        {
            int newStage = 1;
            for (int i = StageThresholds.Length - 1; i >= 0; i--)
            {
                if (Mass.Value >= StageThresholds[i])
                {
                    newStage = i + 1;
                    break;
                }
            }
            CurrentStage.Value = Math.Min(newStage, MaxStage);
        }

        public void Dispose()
        {
            Mass.Dispose();
            CurrentStage.Dispose();
        }
    }
}
