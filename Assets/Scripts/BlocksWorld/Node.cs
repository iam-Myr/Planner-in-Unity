using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public class Node 
{
    private Node parent;
    private WorldState state;
    private Action action; // Action that got us here
    private List<SharedDelegate> unsatisfiedGoals;
    private int depth;

    public Node(Node parent, WorldState state, Action action)
    {
        this.parent = parent;
        this.state = state;
        this.action = action;

        unsatisfiedGoals = state.GetAtoms();

        if (parent == null) depth = 0;
        else  depth = parent.GetDepth() + 1;
    }

    // Node is goal if the current state atoms are a subset of the init state
    public bool isGoal(Node initNode)
    {
        List<SharedDelegate> initPreds = initNode.GetState().GetAtoms();
        List<SharedDelegate> nodePreds = state.GetAtoms();

        foreach (SharedDelegate p in nodePreds)
        {
            if (!ContainsPredicate(initPreds, p))
                return false;
        }
        
        return true;
    }
    
    public bool ContainsPredicate(List<SharedDelegate> pList, SharedDelegate p)
    {
        foreach (SharedDelegate p_list in pList)
        {
            if (p.isSame(p_list))
            {
                return true;
            }

        }
        return false;
    }

    public void Print()
    {
        Debug.Log("================================== ANALYSIS ====================================");
        Debug.Log($"Depth: {depth}");
        Debug.Log($"----------------- Current State ------------------- ");
        if (state != null) state.Print();
        Debug.Log("------------------ Previous Action ----------------- ");
        if (action != null) action.Print();
        Debug.Log($"Unsatisfied Goals: {unsatisfiedGoals.Count}");
        Debug.Log("================================== END ANALYSIS ====================================");
    }

    public void RemoveGoal(SharedDelegate goal) 
    {
        unsatisfiedGoals.Remove(goal);
    }

    public void SetState(WorldState state)
    {
        this.state = state;
    }
    public int GetDepth() => depth;
    public WorldState GetState() => state;
    public Action GetAction() => action;
    public Node GetParent() => parent;

    public List<SharedDelegate> GetUnsatisfiedGoals() => unsatisfiedGoals;
}
