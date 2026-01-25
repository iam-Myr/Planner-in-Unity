using UnityEngine;

public class SmolRaycast : MonoBehaviour
{
    public float rayDistance = 1f;
    public float rayOffset = 0.6f;

    // Only detect objects in this layer (set in inspector)
    private LayerMask blockLayer;


    void Start()
    {
        blockLayer =  LayerMask.GetMask("Block");
        if (blockLayer == 0)
            Debug.LogError("Error: Layer 'Block' does not exist! Please add it in the Unity Editor.");
        // Cast down
        RaycastHit2D down = Physics2D.Raycast(transform.position + Vector3.down * rayOffset, Vector2.down, rayDistance, blockLayer);
        if (down.collider != null)
            Debug.Log($"Down hit: {down.collider.gameObject.name}");
        else
            Debug.Log("Down didn't hit any block.");

        // Cast up
        RaycastHit2D up = Physics2D.Raycast(transform.position + Vector3.up * rayOffset, Vector2.up, rayDistance, blockLayer);
        if (up.collider != null)
            Debug.Log($"Up hit: {up.collider.gameObject.name}");
        else
            Debug.Log("Up didn't hit any block.");
    }

    // Draw rays in the Scene view for debugging
    void OnDrawGizmoss()
    {
        // Down ray (red)
        Vector3 downStart = transform.position + Vector3.down * rayOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(downStart, downStart + Vector3.down * rayDistance);

        // Up ray (green)
        Vector3 upStart = transform.position + Vector3.up * rayOffset;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(upStart, upStart + Vector3.up * rayDistance);
    }
}
