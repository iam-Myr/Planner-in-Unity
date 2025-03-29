using UnityEngine;

public class Block : MonoBehaviour
{
    private Block above;
    private Block below;

    public bool isClear()
    {
        return above == null;
    }

    public Block GetAbove()
    {
        return above;
    }

    public Block GetBelow()
    {
        return below;
    }

    public void SetAbove(Block x)
    {
        above = x;
    }

    public void SetBelow(Block x)
    {
        below = x;
    }

}
