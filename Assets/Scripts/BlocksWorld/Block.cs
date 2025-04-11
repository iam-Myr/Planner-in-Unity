using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public string blockName;
    public Block above;
    public Block below;

    public void MoveTo(Vector3 targetPosition, float speed = 2f)
    {
        StopAllCoroutines(); // In case another movement is happening
        StartCoroutine(MoveSmoothly(targetPosition, speed));
    }

    private IEnumerator MoveSmoothly(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }
        transform.position = targetPosition; // Snap exactly to the target
    }


    public bool isClear() => above == null;
    public Block GetAbove() => above;
    public Block GetBelow() => below;
    public void SetAbove(Block x) => above = x;
    public void SetBelow(Block x) => below = x;
    public override string ToString() => blockName;

}
