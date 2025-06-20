using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlanAction
{
    protected string actionName;
    public List<object> actionArgs = new List<object>();
    protected List<Predicate> preconditions = new List<Predicate>(); 
    protected List<Predicate> effects = new List<Predicate>();

    public PlanAction() { }

    public PlanAction(List<object> args)
    {
        actionArgs = args;
    }

    public virtual PlanAction CreateNew(List<object> args)
    {
        return new PlanAction(args);
    }

    public virtual List<Predicate> InitPreconditions() => new();
    public virtual List<Predicate> InitEffects() => new();
    public virtual async Task Execute(Agent agent) { await Task.CompletedTask; }

    public List<Predicate> GetPreconditions() => preconditions; 
    public List<Predicate> GetEffects() => effects;

    public string Print()
    {
        string[] args = new string[actionArgs.Count];
        for (int i = 0; i < actionArgs.Count; i++)
        {
            args[i] = actionArgs[i]?.ToString() ?? "null";
        }
        return $"{actionName}({string.Join(",", args)})"; // Use string interpolation
    }

    public virtual PlanAction CreateEmpty() => (PlanAction)Activator.CreateInstance(this.GetType());

    public bool IsRemovingGoal(List<Predicate> goals)
    {
        foreach (Predicate effect in effects)
        {
            foreach (Predicate goal in goals)
            {
                if (effect.IsOpposite(goal) &&
                    effect.evaluation != goal.evaluation)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool isValid()
    {
        foreach (Predicate p in preconditions)
        {
            if (!p.EvaluatePredicate()) return false;
        }
        return true;
    }

    /*
    public virtual PlanAction Clone()
    {
        PlanAction clone = (PlanAction)Activator.CreateInstance(this.GetType());
        Dictionary<Pointer, Pointer> varMap = new Dictionary<Pointer, Pointer>();

        clone.actionArgs = new List<Pointer>();
        foreach (Pointer arg in this.actionArgs)
            clone.actionArgs.Add(varMap[arg] = arg.Clone());

        clone.preconditions = new List<Predicate>();
        foreach (Predicate pre in this.preconditions)
        {
            List<Pointer> clonedArgs = new List<Pointer>();
            foreach (Pointer arg in pre.args)
            {
                if (!varMap.ContainsKey(arg)) 
                    varMap[arg] = arg.Clone();
                clonedArgs.Add(varMap[arg]);
            }
            clone.preconditions.Add(new Predicate(pre.func, clonedArgs, pre.evaluation));
        }

        clone.effects = new List<Predicate>();
        foreach (Predicate eff in this.effects)
        {
            List<Pointer> clonedArgs = new List<Pointer>();
            foreach (Pointer arg in eff.args)
            {
                if (!varMap.ContainsKey(arg)) 
                    varMap[arg] = arg.Clone();
                clonedArgs.Add(varMap[arg]);
            }
            clone.effects.Add(new Predicate(eff.func, clonedArgs, eff.evaluation));
        }

        clone.actionName = this.actionName;
        return clone;
    }*/

}
