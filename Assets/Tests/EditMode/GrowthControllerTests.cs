using NUnit.Framework;
using Asteroids.Core;

namespace Asteroids.Tests
{
    public class GrowthControllerTests
    {
        GrowthController _controller;

        [SetUp]
        public void SetUp() => _controller = new GrowthController();

        [TearDown]
        public void TearDown() => _controller.Dispose();

        [Test]
        public void InitialState_Stage1_Mass1()
        {
            Assert.AreEqual(1, _controller.CurrentStage.Value);
            Assert.AreEqual(1f, _controller.Mass.Value, 0.001f);
        }

        [Test]
        public void AddMass_BelowThreshold_StaysStage1()
        {
            _controller.AddMass(5f); // mass = 6, threshold[1]=10
            Assert.AreEqual(1, _controller.CurrentStage.Value);
        }

        [Test]
        public void AddMass_HitsThreshold_AdvancesStage()
        {
            _controller.AddMass(9f); // mass = 10, threshold[1]=10 → stage 2
            Assert.AreEqual(2, _controller.CurrentStage.Value);
        }

        [Test]
        public void AddMass_SkipsThreshold_AdvancesToCorrectStage()
        {
            _controller.AddMass(199f); // mass = 200, threshold[3]=200 → stage 4
            Assert.AreEqual(4, _controller.CurrentStage.Value);
        }

        [Test]
        public void AddMass_ExceedsMax_ClampsToStage5()
        {
            _controller.AddMass(99999f);
            Assert.AreEqual(GrowthController.MaxStage, _controller.CurrentStage.Value);
        }

        [Test]
        public void AddMass_Zero_Ignored()
        {
            float before = _controller.Mass.Value;
            _controller.AddMass(0f);
            Assert.AreEqual(before, _controller.Mass.Value, 0.001f);
        }

        [Test]
        public void SetMass_UpdatesMassAndStage()
        {
            _controller.SetMass(50f); // threshold[2]=50 → stage 3
            Assert.AreEqual(50f, _controller.Mass.Value, 0.001f);
            Assert.AreEqual(3, _controller.CurrentStage.Value);
        }
    }
}
