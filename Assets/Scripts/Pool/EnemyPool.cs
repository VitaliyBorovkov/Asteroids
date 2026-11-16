using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;
using Asteroids.Gameplay;

namespace Asteroids.Pool
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] EnemyAsteroid prefab;
        [SerializeField] int defaultCapacity = 20;
        [SerializeField] int maxSize = 60;

        IObjectResolver _container;
        ObjectPool<EnemyAsteroid> _pool;
        readonly List<EnemyAsteroid> _active = new();

        public IReadOnlyList<EnemyAsteroid> Active => _active;

        [Inject]
        void Construct(IObjectResolver container) => _container = container;

        void Awake()
        {
            _pool = new ObjectPool<EnemyAsteroid>(
                createFunc: Create,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: e => Destroy(e.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        public EnemyAsteroid Spawn(Vector3 position, Vector3 velocity)
        {
            var enemy = _pool.Get();
            enemy.transform.SetPositionAndRotation(position, Quaternion.identity);
            enemy.Launch(velocity);
            return enemy;
        }

        public void Despawn(EnemyAsteroid enemy) => _pool.Release(enemy);

        EnemyAsteroid Create()
        {
            var obj = Instantiate(prefab);
            _container.InjectGameObject(obj.gameObject);
            obj.gameObject.SetActive(false);
            return obj;
        }

        void OnGet(EnemyAsteroid e)
        {
            _active.Add(e);
            e.gameObject.SetActive(true);
        }

        void OnRelease(EnemyAsteroid e)
        {
            _active.Remove(e);
            e.gameObject.SetActive(false);
        }
    }
}
