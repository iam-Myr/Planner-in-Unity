using UnityEngine;

public class Area : MonoBehaviour
{
    public string areaName;
    public Collider2D areaCollider;

    public Vector3 GetPosition() => transform.position;

    public bool Contains(Transform target)
    {
        // Check if the target's position lies inside the bounding box of the areaCollider
        return areaCollider.bounds.Contains(target.position);
    }

}
