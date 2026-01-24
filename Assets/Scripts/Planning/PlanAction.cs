using System;
using System.Collections;
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
        protected Func<List<object>, IEnumerator> executable;
        public float durationEstimate { get; protected set; } = 20f; // fallback

        public enum ActionStatus
        {
            NotStarted,
            InProgress,
            Completed,
            Failed,
            Interrupted
        }
        public ActionStatus Status { get; protected set; } = ActionStatus.NotStarted;


        public PlanAction()
        {
            preconditions.AddRange(InitPreconditions());
            effects.AddRange(InitEffects());
        }

        // Template
        public PlanAction AddExecutable(Func<List<object>, IEnumerator> coroutineExecutable, float duration)
        {
            this.executable = coroutineExecutable;
            this.actionName = coroutineExecutable?.Method.Name ?? "UnnamedAction";
            this.durationEstimate = duration;
            return this;
        }


        public abstract PlanAction CreateNew(List<Pointer> args); 

        public abstract List<Predicate> InitPreconditions();
        public abstract List<Predicate> InitEffects();


        public virtual float GetEstimatedDuration() => durationEstimate;

        public List<Predicate> GetPreconditions() => preconditions;
        public List<Predicate> GetEffects() => effects;
        public ActionStatus GetStatus() => Status;


        public override string ToString()
        {
            string[] args = new string[actionArgs.Count];
            for (int i = 0; i < actionArgs.Count; i++)
            {
                args[i] = actionArgs[i]?.Get()?.ToString() ?? "null";
            }
            return $"{actionName}({string.Join(",", args)})"; // Use string interpolation
        }

        public List<Type> GetArgTypes()
        {
            List<Type> types = new List<Type>();
            foreach(Pointer arg in actionArgs) 
            { 
                types.Add(arg.GetDeclaredType());
            }

            return types;
        }


        public bool IsRemovingGoal(List<Predicate> goals)
        {
            foreach (Predicate effect in effects)
            {
                foreach (Predicate goal in goals)
                {
                    if (effect.IsOpposite(goal) &&
                        effect.Value != goal.Value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsValid()
        {
            foreach(Predicate p  in preconditions)
            {
                if (!p.Evaluate()) return false;
            }
            return true;
        }




        public virtual IEnumerator Execute()
        {
            if (executable == null)
            {
                Debug.LogWarning($"Action '{actionName}' has no coroutine executable assigned.");
                Status = ActionStatus.Failed;
                yield break;
            }

            // Start executing action
            Status = ActionStatus.InProgress;

            List<object> argsValues = actionArgs.ConvertAll(arg => arg.Get());
            IEnumerator coroutine = executable(argsValues);

            bool interrupted = false;
            while (true)
            {
                if (Status == ActionStatus.Interrupted)
                {
                    Debug.Log($"Action '{actionName}' interrupted.");
                    interrupted = true;
                    break;
                }

                if (!coroutine.MoveNext())
                    break;

                yield return coroutine.Current;
            }

            if (interrupted)
            {
                Status = ActionStatus.Failed;
                yield break;
            }

            // Mark as complete only if not interrupted
            Status = ActionStatus.Completed;
        }


        public void Cancel()
        {
            if (Status == ActionStatus.InProgress)
            {
                Status = ActionStatus.Interrupted;
            }
        }

        public virtual PlanAction Clone()
        {
            // Create a new instance of the same action type
            PlanAction clone = (PlanAction)Activator.CreateInstance(this.GetType());

            // Map original pointers to cloned pointers to preserve shared logical variables
            Dictionary<Pointer, Pointer> pointerMap = new Dictionary<Pointer, Pointer>();

            // Clone action arguments
            clone.actionArgs = new List<Pointer>();
            foreach (Pointer arg in this.actionArgs)
            {
                Pointer clonedArg = arg.Clone();
                pointerMap[arg] = clonedArg;
                clone.actionArgs.Add(clonedArg);
            }

            // Clone preconditions
            clone.preconditions = new List<Predicate>();
            foreach (Predicate pre in this.preconditions)
            {
                List<Pointer> clonedArgs = new List<Pointer>();
                foreach (Pointer arg in pre.Args)
                {
                    if (!pointerMap.ContainsKey(arg))
                        pointerMap[arg] = arg.Clone();

                    clonedArgs.Add(pointerMap[arg]);
                }
                clone.preconditions.Add(new Predicate(pre.TheFunc, clonedArgs, pre.Value ?? false));
            }

            // Clone effects
            clone.effects = new List<Predicate>();
            foreach (Predicate eff in this.effects)
            {
                List<Pointer> clonedArgs = new List<Pointer>();
                foreach (Pointer arg in eff.Args)
                {
                    if (!pointerMap.ContainsKey(arg))
                        pointerMap[arg] = arg.Clone();

                    clonedArgs.Add(pointerMap[arg]);
                }
                clone.effects.Add(new Predicate(eff.TheFunc, clonedArgs, eff.Value ?? false));
            }

            // Copy action name
            clone.actionName = this.actionName;

            // Copy duration and executable if needed
            clone.durationEstimate = this.durationEstimate;
            clone.executable = this.executable;

            return clone;
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