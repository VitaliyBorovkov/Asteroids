using UnityEngine;
using VContainer;
using Asteroids.Pool;

namespace Asteroids.Gameplay
{
    /// <summary>
    /// Keeps the player near the world origin by shifting all active objects
    /// when the player drifts beyond the threshold. Creates an infinite-world illusion.
    /// </summary>
    public class WorldOriginShifter : MonoBehaviour
    {
        [SerializeField] float shiftThreshold = 20f;

        PlayerAsteroid _player;
        EnemyPool _enemyPool;
        DustPool _dustPool;
        Camera _camera;

        [Inject]
        void Construct(PlayerAsteroid player, EnemyPool enemyPool, DustPool dustPool)
        {
            _player = player;
            _enemyPool = enemyPool;
            _dustPool = dustPool;
        }

        void Start()
        {
            _camera = Camera.main;
        }

        void LateUpdate()
        {
            var pos = _player.transform.position;
            if (Mathf.Abs(pos.x) < shiftThreshold && Mathf.Abs(pos.z) < shiftThreshold)
                return;

            var shift = new Vector3(-pos.x, 0f, -pos.z);

            _player.Shift(shift);

            foreach (var enemy in _enemyPool.Active)
                ShiftRigidbody(enemy.transform, shift);

            foreach (var dust in _dustPool.Active)
                dust.transform.position += shift;

            if (_camera != null)
                _camera.transform.position += shift;
        }

        static void ShiftRigidbody(Transform t, Vector3 delta)
        {
            if (t.TryGetComponent<Rigidbody>(out var rb))
                rb.position += delta;
            else
                t.position += delta;
        }
    }
}
