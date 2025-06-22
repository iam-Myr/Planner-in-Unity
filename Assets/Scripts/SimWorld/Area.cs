using UnityEngine;
using Planning;

namespace SimWorld
{
    public class Area : PlanObject
    {
        public string areaName;
        private Collider2D areaCollider;

        private void Start()
        {
            areaCollider = GetComponent<Collider2D>();
        }
        public Vector3 GetPosition() => transform.position;

        public bool Contains(Vector3 targetPos)
        {
            // Check if the target's position lies inside the bounding box of the areaCollider
            return areaCollider.bounds.Contains(targetPos);
        }

    }
}
