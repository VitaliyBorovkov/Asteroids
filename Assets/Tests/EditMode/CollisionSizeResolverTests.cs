using NUnit.Framework;
using Asteroids.Collision;

namespace Asteroids.Tests
{
    public class CollisionSizeResolverTests
    {
        CollisionSizeResolver _resolver;

        [SetUp]
        public void SetUp() => _resolver = new CollisionSizeResolver(step: 0.5f);

        // Equal mass → 1 hit
        [Test]
        public void EqualMass_Returns1()
            => Assert.AreEqual(1, _resolver.GetHitsToBreak(5f, 5f));

        // Target twice as heavy → ratio=2, (2-1)/0.5=2, hits=3
        [Test]
        public void TargetTwiceAsHeavy_Returns3()
            => Assert.AreEqual(3, _resolver.GetHitsToBreak(5f, 10f));

        // Target lighter than player → 1 hit minimum
        [Test]
        public void TargetLighter_Returns1()
            => Assert.AreEqual(1, _resolver.GetHitsToBreak(10f, 3f));

        // Ratio 1.3 → (0.3/0.5)=0.6, floor=0, hits=1
        [Test]
        public void SlightlyHeavier_Returns1()
            => Assert.AreEqual(1, _resolver.GetHitsToBreak(10f, 13f));

        // Ratio 1.5 → (0.5/0.5)=1, hits=2
        [Test]
        public void Ratio1_5_Returns2()
            => Assert.AreEqual(2, _resolver.GetHitsToBreak(10f, 15f));

        // IsBreakable always returns true (design decision)
        [Test]
        public void IsBreakable_AlwaysTrue()
        {
            Assert.IsTrue(_resolver.IsBreakable(1f, 9999f));
            Assert.IsTrue(_resolver.IsBreakable(9999f, 1f));
        }

        [Test]
        public void InvalidStep_Throws()
            => Assert.Throws<System.ArgumentException>(() => new CollisionSizeResolver(step: 0f));

        [Test]
        public void InvalidPlayerMass_Throws()
            => Assert.Throws<System.ArgumentException>(() => _resolver.GetHitsToBreak(0f, 5f));
    }
}
