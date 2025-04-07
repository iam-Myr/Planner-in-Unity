using UnityEngine;

public class Block : MonoBehaviour
{
    public string blockName;
    private Block above;
    private Block below;

    public bool isClear() => above == null;
    public Block GetAbove() => above;
    public Block GetBelow() => below;
    public void SetAbove(Block x) => above = x;
    public void SetBelow(Block x) => below = x;
    public override string ToString() => blockName;

}
