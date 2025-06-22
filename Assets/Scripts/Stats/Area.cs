using UnityEngine;

namespace SimWorld
{
    public class Area : MonoBehaviour
    {
        public string areaName;
        private Collider2D areaCollider;

        private void Start()
        {
            areaCollider = GetComponent<Collider2D>();
        }
        public Vector3 GetPosition() => transform.position;

        public bool Contains(Transform target)
        {
            // Check if the target's position lies inside the bounding box of the areaCollider
            return areaCollider.bounds.Contains(target.position);
        }

    }
}
