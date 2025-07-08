using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Planning
{
    public abstract class PlanAction
    {
        protected string actionName;
        public List<Pointer> actionArgs = new List<Pointer>();
        protected List<Predicate> preconditions = new List<Predicate>();
        protected List<Predicate> effects = new List<Predicate>();
        protected Action<List<object>> executable ;

        public PlanAction()
        {
            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        // Template
        public void AddExecutable(Action<List<object>> executable)
        {
            this.executable = executable;
            actionName = executable.Method.Name;
        }

        public abstract PlanAction CreateNew(List<Pointer> args); // Instantiation
        
            //PlanAction a = new PlanAction(preconditions, effects, executable);
            //a.actionArgs = new List<Pointer>(args);
           // return a;
        

        public abstract List<Predicate> InitPreconditions();
        public abstract List<Predicate> InitEffects();
        public virtual async Task Execute(object arg) { await Task.CompletedTask; }

        public List<Predicate> GetPreconditions() => preconditions;
        public List<Predicate> GetEffects() => effects;

        public override string ToString()
        {
            string[] args = new string[actionArgs.Count];
            for (int i = 0; i < actionArgs.Count; i++)
            {
                args[i] = actionArgs[i]?.Get()?.ToString() ?? "null";
            }
            return $"{actionName}({string.Join(",", args)})"; // Use string interpolation
        }

        public bool IsRemovingGoal(List<Predicate> goals)
        {
            foreach (Predicate effect in effects)
            {
                foreach (Predicate goal in goals)
                {
                    if (effect.IsOpposite(goal) &&
                        effect.value != goal.value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsValid()
        {
            return true;
        }

        // Executes the action's executable with the action args as parameters
        public void Execute() 
        {
            List<object> argsValues = new List<object>();
            foreach (Pointer arg in actionArgs) { argsValues.Add(arg.Get()); }
            executable(argsValues);
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
                clone.preconditions.Add(new Predicate(pre.func, clonedArgs, pre.value));
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
                clone.effects.Add(new Predicate(eff.func, clonedArgs, eff.value));
            }

            clone.actionName = this.actionName;
            return clone;
        }

        public virtual PlanAction CreateEmpty() => (PlanAction)Activator.CreateInstance(this.GetType());

        // Cool ChatGPT code probably super inefficient 
        public virtual PlanAction Clone1()
        {
            PlanAction clone = new PlanAction();

            // Args
            clone.actionArgs = new List<Pointer>();
            foreach (Pointer arg in actionArgs)
                clone.actionArgs.Add(arg.Clone());

            // Precnditions
            clone.preconditions = new List<Predicate>();
            foreach (Predicate arg in preconditions)
                clone.preconditions.Add(arg.Clone());

            // Effects
            clone.effects = new List<Predicate>();
            foreach (Predicate arg in effects)
                clone.effects.Add(arg.Clone());

            clone.actionName = actionName;

            return clone;

            //// Registry to keep same logical variable shared
            //Dictionary<SharedVar, SharedVar> varMap = new Dictionary<SharedVar, SharedVar>();

            //// Clone actionArgs and store in registry
            //newAction.actionArgs = this.actionArgs.Select(arg =>
            //{
            //    var cloned = arg.Clone();
            //    varMap[arg] = cloned;
            //    return cloned;
            //}).ToArray();

            //// Clone preconditions with shared vars
            //newAction.preconditions = this.preconditions
            //    .Select(p => (
            //        p.Item1,
            //        p.Item2.Select(v =>
            //        {
            //            if (!varMap.ContainsKey(v))
            //                varMap[v] = v.Clone();
            //            return varMap[v];
            //        }).ToArray()
            //    )).ToList();

            //// Clone effects with shared vars
            //newAction.effects = this.effects
            //    .Select(e => (
            //        e.Item1,
            //        e.Item2.Select(v =>
            //        {
            //            if (!varMap.ContainsKey(v))
            //                varMap[v] = v.Clone();
            //            return varMap[v];
            //        }).ToArray()
            //    )).ToList();


        }

        public bool IsUseful(Node node)
        {
            var unsatisfiedGoals = node.GetUnsatisfiedGoals();
            foreach (Predicate goal in unsatisfiedGoals)
            {
                if (effects.Any(effect => Unification.CanUnify(effect, goal)))
                {
                    return true;
                }
            }
            return false;
        } 

        /*
        public List<Predicate> SatisfyOtherGoals(List<Predicate> unsatisfiedGoals)
        {
            List<Predicate> satisfiedGoals = new List<Predicate>();

            foreach (Predicate effect in effects)
            {
                foreach (Predicate goal in unsatisfiedGoals)
                {
                    // Check if goal is fully instantiated and matches the effect
                    if (goal.IsInstantiated() && effect.Equals(goal))
                    {
                        satisfiedGoals.Add(goal);
                    }
                }
            }

            return satisfiedGoals;
        }
        */


    }
}