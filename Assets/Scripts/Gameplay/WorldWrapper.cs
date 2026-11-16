using UnityEngine;

namespace Asteroids.Gameplay
{
    /// <summary>
    /// Wraps object to the opposite side when it crosses the play area boundary.
    /// </summary>
    public class WorldWrapper : MonoBehaviour
    {
        [SerializeField] float bounds = 28f;

        void LateUpdate()
        {
            Vector3 pos = transform.position;
            if (pos.x > bounds) pos.x = -bounds;
            else if (pos.x < -bounds) pos.x = bounds;
            if (pos.z > bounds) pos.z = -bounds;
            else if (pos.z < -bounds) pos.z = bounds;
            transform.position = pos;
        }
    }
}
