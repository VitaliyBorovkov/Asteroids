using UnityEngine;
using VContainer;
using Asteroids.Core;
using Asteroids.Pool;

namespace Asteroids.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class DustPickup : MonoBehaviour
    {
        [SerializeField] float amount = 1f;

        UpgradeManager _upgradeManager;
        DustPool _pool;
        bool _isDespawning;

        [Inject]
        void Construct(UpgradeManager upgradeManager, DustPool pool)
        {
            _upgradeManager = upgradeManager;
            _pool = pool;
        }

        // Called by EnemyAsteroid before spawning to set custom dust value
        public void SetAmount(float value) => amount = value;

        void OnEnable() => _isDespawning = false;

        void OnTriggerEnter(Collider other)
        {
            if (_isDespawning) return;
            if (!other.TryGetComponent<PlayerAsteroid>(out _)) return;

            _isDespawning = true;
            _upgradeManager.AddDust(amount);
            _pool.Despawn(this);
        }
    }
}
