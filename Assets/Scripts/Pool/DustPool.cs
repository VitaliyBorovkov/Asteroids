using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;
using Asteroids.Gameplay;

namespace Asteroids.Pool
{
    public class DustPool : MonoBehaviour
    {
        [SerializeField] DustPickup prefab;
        [SerializeField] int defaultCapacity = 50;
        [SerializeField] int maxSize = 200;

        IObjectResolver _container;
        ObjectPool<DustPickup> _pool;
        readonly List<DustPickup> _active = new();

        public IReadOnlyList<DustPickup> Active => _active;

        [Inject]
        void Construct(IObjectResolver container) => _container = container;

        void Awake()
        {
            _pool = new ObjectPool<DustPickup>(
                createFunc: Create,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: d => Destroy(d.gameObject),
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        public DustPickup Spawn(Vector3 position)
        {
            var dust = _pool.Get();
            dust.transform.SetPositionAndRotation(position, Quaternion.identity);
            return dust;
        }

        public void Despawn(DustPickup dust) => _pool.Release(dust);

        DustPickup Create()
        {
            var obj = Instantiate(prefab);
            _container.InjectGameObject(obj.gameObject);
            obj.gameObject.SetActive(false);
            return obj;
        }

        void OnGet(DustPickup d)
        {
            _active.Add(d);
            d.gameObject.SetActive(true);
        }

        void OnRelease(DustPickup d)
        {
            _active.Remove(d);
            d.gameObject.SetActive(false);
        }
    }
}
