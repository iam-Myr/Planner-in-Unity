using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public class Node 
{
    private Node parent;
    private WorldState state;
    private Action action; // Action that got us here
    private List<Predicate> unsatisfiedGoals;
    private int depth;

    public Node(Node parent, WorldState state, Action action)
    {
        this.parent = parent;
        this.state = state;
        this.action = action;

        unsatisfiedGoals = state.GetPredicates();

        if (parent == null) depth = 0;
        else  depth = parent.GetDepth() + 1;
    }

    // Node is goal if the current state atoms are a subset of the init state
    public bool isGoal(Node initNode)
    {
        List<Predicate> initPreds = initNode.GetState().GetPredicates();
        List<Predicate> nodePreds = state.GetPredicates();

        foreach (Predicate p in nodePreds)
        {
            if (!ContainsPredicate(initPreds, p))
                return false;
        }
        
        return true;
    }
    
    public bool ContainsPredicate(List<Predicate> pList, Predicate p)
    {
        foreach (Predicate p_list in pList)
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
        //Debug.Log($"----------------- Current State ------------------- ");
        //if (state != null) state.Print();
        Debug.Log($"----------------- Unsatisfied Goals ------------------- ");
        if (unsatisfiedGoals != null) PrintGoals();
        Debug.Log($"Remaining Goals: {unsatisfiedGoals.Count}");
        Debug.Log("------------------ Previous Action ----------------- ");
        if (action != null) Debug.Log(action.Print());
        Debug.Log("================================== END ANALYSIS ====================================");
    }

    public void PrintGoals()
    {
        foreach (Predicate p in unsatisfiedGoals) { Debug.Log(p.ToString()); }
    }

    public void RemoveGoal(Predicate goal) 
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

    public List<Predicate> GetUnsatisfiedGoals() => unsatisfiedGoals;
}
