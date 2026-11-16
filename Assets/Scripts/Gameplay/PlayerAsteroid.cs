using UnityEngine;
using UnityEngine.InputSystem;

namespace Asteroids.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAsteroid : MonoBehaviour
    {
        [SerializeField] float speed = 8f;
        [SerializeField] float mass = 5f;
        [SerializeField] float arrivalThreshold = 0.15f;

        public float Mass => mass;

        Rigidbody _rb;
        Vector3 _targetPosition;
        bool _hasTarget;

        readonly Plane _groundPlane = new(Vector3.up, Vector3.zero);

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.isKinematic = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _rb.constraints = RigidbodyConstraints.FreezePositionY
                | RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ;
            _targetPosition = transform.position;
        }

        void Update()
        {
            if (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasReleasedThisFrame)
                TrySetTarget();
        }

        void FixedUpdate()
        {
            if (!_hasTarget) return;

            Vector3 toTarget = _targetPosition - transform.position;
            toTarget.y = 0f;

            if (toTarget.magnitude <= arrivalThreshold)
            {
                _rb.linearVelocity = Vector3.zero;
                _hasTarget = false;
                return;
            }

            _rb.linearVelocity = toTarget.normalized * speed;
        }

        void TrySetTarget()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (_groundPlane.Raycast(ray, out float distance))
            {
                _targetPosition = ray.GetPoint(distance);
                _targetPosition.y = 0f;
                _hasTarget = true;
            }
        }

        public void Shift(Vector3 delta)
        {
            _rb.position += delta;
            _targetPosition += delta;
        }

        void OnCollisionEnter(UnityEngine.Collision col)
        {
            if (col.gameObject.TryGetComponent<EnemyAsteroid>(out var enemy))
                enemy.TakeHit(mass);
        }
    }
}
