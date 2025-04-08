using UnityEngine;
using System.Linq;


public class Node 
{
    private Node parent;
    private WorldState state;
    private Action action; // Action that got us here
    //private List<(Func<object[], bool>, SharedVar[])> goalSet; NEXT TIME, a set of unsatisfied goals
    private int depth;

    // add satisfied and unsatisfied goal atoms maybe

    public Node(Node parent, WorldState state, Action action)
    {
        this.parent = parent;
        this.state = state;
        this.action = action;

        if (parent == null) depth = 0;
        else  depth = parent.GetDepth() + 1;
    }

    // Node is goal if the goal's atoms are a subset of current state atoms
    public bool isGoal(Node goalNode)
    {
        WorldState goalState = goalNode.GetState();
        return goalState.GetAtoms().All(i => state.GetAtoms().Contains(i));
    }

    public void Print()
    {
        Debug.Log("================== ANALYSIS =========================");
        Debug.Log($"Depth {depth}");
        Debug.Log($"----------------- Current State ------------------- ");
        if (state != null) state.Print();
        Debug.Log("------------------ Previous Action ----------------- ");
        if (action != null) action.Print();
        Debug.Log("");
    }
    public void SetState(WorldState state)
    {
        this.state = state;
    }
    public int GetDepth() => depth;
    public WorldState GetState() => state;
    public Action GetAction() => action;
    public Node GetParent() => parent;
}
