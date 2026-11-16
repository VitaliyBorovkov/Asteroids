using UnityEngine;
using VContainer;
using Asteroids.Collision;
using Asteroids.Core;
using Asteroids.Pool;

namespace Asteroids.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyAsteroid : MonoBehaviour
    {
        [SerializeField] float mass = 3f;
        [SerializeField] float dustAmountPerPickup = 1f;
        [SerializeField] int dustSpawnCount = 5;
        [SerializeField] float dustScatterRadius = 1f;

        public float Mass => mass;

        UpgradeManager _upgradeManager;
        CollisionSizeResolver _resolver;
        EnemyPool _enemyPool;
        DustPool _dustPool;

        Rigidbody _rb;
        int _hitsReceived;
        int _hitsRequired;
        bool _hitsInitialized;
        bool _isDespawning;

        [Inject]
        void Construct(
            UpgradeManager upgradeManager,
            CollisionSizeResolver resolver,
            EnemyPool enemyPool,
            DustPool dustPool)
        {
            _upgradeManager = upgradeManager;
            _resolver = resolver;
            _enemyPool = enemyPool;
            _dustPool = dustPool;
        }

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.isKinematic = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _rb.constraints = RigidbodyConstraints.FreezePositionY
                | RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ;
        }

        void OnEnable()
        {
            _hitsReceived = 0;
            _hitsRequired = 0;
            _hitsInitialized = false;
            _isDespawning = false;
        }

        public void Launch(Vector3 velocity)
        {
            _rb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
        }

        void OnDisable()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        public void TakeHit(float attackerMass)
        {
            if (_isDespawning) return;

            if (!_hitsInitialized)
            {
                _hitsRequired = _resolver.GetHitsToBreak(attackerMass, mass);
                _hitsInitialized = true;
            }

            _hitsReceived++;

            if (_hitsReceived >= _hitsRequired)
                DestroyAndSpawnDust();
        }

        void OnCollisionEnter(UnityEngine.Collision col)
        {
            if (_isDespawning) return;
            if (!col.gameObject.TryGetComponent<EnemyAsteroid>(out _)) return;

            if (col.contactCount == 0) return;
            var normal = col.GetContact(0).normal;
            normal.y = 0f;
            if (normal.sqrMagnitude < 0.001f) return;

            var vel = _rb.linearVelocity;
            vel.y = 0f;
            _rb.linearVelocity = Vector3.Reflect(vel, normal.normalized);
        }

        void DestroyAndSpawnDust()
        {
            _isDespawning = true;

            for (int i = 0; i < dustSpawnCount; i++)
            {
                Vector2 circle = Random.insideUnitCircle * dustScatterRadius;
                var offset = new Vector3(circle.x, 0f, circle.y);
                var dust = _dustPool.Spawn(transform.position + offset);
                dust.SetAmount(dustAmountPerPickup);
            }

            _enemyPool.Despawn(this);
        }
    }
}
