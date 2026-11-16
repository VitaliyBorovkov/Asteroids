using NUnit.Framework;
using Asteroids.Core;

namespace Asteroids.Tests
{
    public class UpgradeManagerTests
    {
        UpgradeManager _manager;

        [SetUp]
        public void SetUp() => _manager = new UpgradeManager();

        [TearDown]
        public void TearDown() => _manager.Dispose();

        [Test]
        public void InitialDust_IsZero()
            => Assert.AreEqual(0f, _manager.Dust.Value, 0.001f);

        [Test]
        public void AddDust_IncreasesBalance()
        {
            _manager.AddDust(10f);
            Assert.AreEqual(10f, _manager.Dust.Value, 0.001f);
        }

        [Test]
        public void AddDust_Zero_Ignored()
        {
            _manager.AddDust(0f);
            Assert.AreEqual(0f, _manager.Dust.Value, 0.001f);
        }

        [Test]
        public void SpendDust_SufficientBalance_ReturnsTrueAndDeducts()
        {
            _manager.AddDust(20f);
            bool result = _manager.SpendDust(10f);
            Assert.IsTrue(result);
            Assert.AreEqual(10f, _manager.Dust.Value, 0.001f);
        }

        [Test]
        public void SpendDust_InsufficientBalance_ReturnsFalseNoChange()
        {
            _manager.AddDust(5f);
            bool result = _manager.SpendDust(10f);
            Assert.IsFalse(result);
            Assert.AreEqual(5f, _manager.Dust.Value, 0.001f);
        }

        [Test]
        public void SpendDust_ExactAmount_LeavesZero()
        {
            _manager.AddDust(15f);
            bool result = _manager.SpendDust(15f);
            Assert.IsTrue(result);
            Assert.AreEqual(0f, _manager.Dust.Value, 0.001f);
        }
    }
}
