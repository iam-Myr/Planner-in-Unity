using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node 
{
    private Node parent;
    private WorldState state;
    private Action action; // Action that got us here
    private List<Predicate> unsatisfiedGoals; // init might not actually achieve it
    protected int depth;

    public Node(Node parent, WorldState state, Action action)
    {
        this.parent = parent;
        this.state = state;
        this.action = action;

        // Init goals

        unsatisfiedGoals = new List<Predicate>(state.GetPredicates());

        if (parent == null) depth = 0;
        else  depth = parent.depth + 1;
    }

    // Node is goal if the current state atoms are a subset of the init state
    public bool isGoal(Node initNode)
    {
        List<Predicate> initPreds = initNode.GetState().GetPredicates();
        List<Predicate> nodePreds = this.state.GetPredicates();

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
            if (p.Equals(p_list))
            {
                return true;
            }

        }
        return false;
    }

    public bool HasSameState(Node other)
    {
        var thisPreds = this.GetState().GetPredicates();
        var otherPreds = other.GetState().GetPredicates();

        if (thisPreds.Count != otherPreds.Count)
            return false;

        foreach (Predicate p in thisPreds)
        {
            if (!otherPreds.Any(a => a.Equals(p)))
                return false;
        }

        return true;
    }

    public bool HasSameGoals(Node other)
    {
        if (unsatisfiedGoals.Count != other.GetUnsatisfiedGoals().Count)
            return false;

        foreach (Predicate g in unsatisfiedGoals)
        {
            if (!other.GetUnsatisfiedGoals().Any(a => a.Equals(g)))
                return false;
        }
        return true;
    }


    public void Print()
    {
        Debug.Log("================================== ANALYSIS ====================================");
        Debug.Log($"Depth: {depth}");
        Debug.Log("------------------ Previous Action ----------------- ");
        if (action != null) Debug.Log(action.Print());
        Debug.Log($"----------------- Current State ------------------- ");
        if (state != null) state.Print();
        Debug.Log($"----------------- Unsatisfied Goals ------------------- ");
        if (unsatisfiedGoals != null) PrintGoals();
        Debug.Log($"Remaining Goals: {unsatisfiedGoals.Count}");
        Debug.Log("================================== END ANALYSIS ====================================");
    }

    public List<Predicate> GetInstantiatedGoals()
    {
        List<Predicate> instantiatedGoals = new List<Predicate>();

        // Iterate over all unsatisfied goals and check if they are instantiated
        foreach (Predicate goal in unsatisfiedGoals)
        {
            if (goal.IsInstantiated()) // Check if all arguments are non-null
            {
                instantiatedGoals.Add(goal);
            }
        }

        return instantiatedGoals;
    }

    public bool isContradiction() => state.IsContradiction();

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
