using VContainer;
using VContainer.Unity;
using Asteroids.Core;
using Asteroids.Collision;
using Asteroids.Gameplay;
using Asteroids.Pool;

namespace Asteroids.DI
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Stateful services — Scoped: fresh instance per scene load, replaceable in tests
            builder.Register<GrowthController>(Lifetime.Scoped);
            builder.Register<UpgradeManager>(Lifetime.Scoped);

            // Stateless logic — single shared instance (no mutable state)
            builder.RegisterInstance(new CollisionSizeResolver(step: 0.5f));

            // Scene MonoBehaviour components
            builder.RegisterComponentInHierarchy<PlayerAsteroid>();
            builder.RegisterComponentInHierarchy<AsteroidSpawner>();
            builder.RegisterComponentInHierarchy<EnemyPool>();
            builder.RegisterComponentInHierarchy<DustPool>();
            builder.RegisterComponentInHierarchy<WorldOriginShifter>();
        }
    }
}
