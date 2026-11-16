using UnityEngine;
using VContainer;
using Asteroids.Pool;

namespace Asteroids.Gameplay
{
    /// <summary>
    /// Maintains a live pool of enemy asteroids within the camera view.
    /// All enemies fly in a shared field direction with random drift per-spawn.
    /// Out-of-range enemies are despawned; new ones spawn inside the visible area.
    /// </summary>
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] int keepAliveCount = 15;
        [SerializeField] float despawnDistance = 45f;
        [SerializeField] float fieldDirectionAngle = 225f;  // degrees: 0=+X, 90=+Z
        [SerializeField] float fieldDriftAngle = 30f;       // max random deviation in degrees
        [SerializeField] float enemySpeed = 3f;

        EnemyPool _enemyPool;
        Camera _camera;
        readonly Plane _groundPlane = new(Vector3.up, Vector3.zero);

        [Inject]
        void Construct(EnemyPool enemyPool) => _enemyPool = enemyPool;

        void Start()
        {
            _camera = Camera.main;
            for (int i = 0; i < keepAliveCount; i++)
                SpawnEnemy();
        }

        void Update()
        {
            var active = _enemyPool.Active;
            var camPos2D = new Vector2(_camera.transform.position.x, _camera.transform.position.z);

            for (int i = active.Count - 1; i >= 0; i--)
            {
                var pos = active[i].transform.position;
                float dist = Vector2.Distance(new Vector2(pos.x, pos.z), camPos2D);
                if (dist > despawnDistance)
                    _enemyPool.Despawn(active[i]);
            }

            int missing = keepAliveCount - _enemyPool.Active.Count;
            for (int i = 0; i < missing; i++)
                SpawnEnemy();
        }

        void SpawnEnemy()
        {
            _enemyPool.Spawn(GetSpawnPosition(), GetLaunchVelocity());
        }

        Vector3 GetSpawnPosition()
        {
            GetViewRect(out float minX, out float maxX, out float minZ, out float maxZ);
            return new Vector3(Random.Range(minX, maxX), 0f, Random.Range(minZ, maxZ));
        }

        Vector3 GetLaunchVelocity()
        {
            float drift = Random.Range(-fieldDriftAngle, fieldDriftAngle);
            float angle = (fieldDirectionAngle + drift) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * enemySpeed;
        }

        void GetViewRect(out float minX, out float maxX, out float minZ, out float maxZ)
        {
            minX = float.MaxValue; maxX = float.MinValue;
            minZ = float.MaxValue; maxZ = float.MinValue;

            for (int i = 0; i < 4; i++)
            {
                float u = (i & 1) == 0 ? 0f : 1f;
                float v = (i & 2) == 0 ? 0f : 1f;
                var ray = _camera.ViewportPointToRay(new Vector3(u, v, 0f));
                if (!_groundPlane.Raycast(ray, out float dist)) continue;
                var p = ray.GetPoint(dist);
                minX = Mathf.Min(minX, p.x); maxX = Mathf.Max(maxX, p.x);
                minZ = Mathf.Min(minZ, p.z); maxZ = Mathf.Max(maxZ, p.z);
            }
        }
    }
}
