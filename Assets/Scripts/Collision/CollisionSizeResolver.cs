using System;

namespace Asteroids.Collision
{
    /// <summary>
    /// Pure logic, no Unity dependency — fully unit-testable.
    /// Formula: hits = 1 + floor((ratio - 1) / step), ratio = targetMass / playerMass
    /// </summary>
    public class CollisionSizeResolver
    {
        public float Step { get; }

        public CollisionSizeResolver(float step = 0.5f)
        {
            if (step <= 0f) throw new ArgumentException("Step must be positive.", nameof(step));
            Step = step;
        }

        public int GetHitsToBreak(float playerMass, float targetMass)
        {
            if (playerMass <= 0f) throw new ArgumentException("playerMass must be positive.", nameof(playerMass));
            float ratio = targetMass / playerMass;
            int hits = 1 + (int)Math.Floor((ratio - 1.0) / Step);
            return Math.Max(1, hits);
        }

        // Per design: every asteroid is breakable — only the hit count differs.
        public bool IsBreakable(float playerMass, float targetMass) => true;
    }
}
