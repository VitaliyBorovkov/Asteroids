using UnityEngine;

namespace Asteroids.Gameplay
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float smoothTime = 0.2f;

        Vector3 _offset;
        Vector3 _velocity;

        void Start()
        {
            if (target != null)
                _offset = transform.position - new Vector3(target.position.x, 0f, target.position.z);
        }

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = new Vector3(target.position.x, 0f, target.position.z) + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        }
    }
}
