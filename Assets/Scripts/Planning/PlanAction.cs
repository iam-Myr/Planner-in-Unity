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
<<<<<<< Updated upstream
        actionArgs = args;
=======
        public string actionName;
        public List<Pointer> actionArgs = new List<Pointer>();
        protected List<Predicate> preconditions = new List<Predicate>();
        protected List<Predicate> effects = new List<Predicate>();
        protected Func<List<object>, IEnumerator> executable;
        public float durationEstimate { get; protected set; } = 20f; // fallback

        private List<Action<List<Pointer>>> constraints
    = new List<Action<List<Pointer>>>();

        public enum ActionStatus
        {
            NotStarted,
            InProgress,
            Completed,
            Failed,
            Interrupted
        }

        public enum FailureReason
        {
            None,
            MissingExecutable,
            PreconditionsInvalid,
            Timeout,
            EffectsNotHolding
        }

        public ActionStatus Status { get; protected set; } = ActionStatus.NotStarted;
        public FailureReason Failure { get; protected set; } = FailureReason.None;



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
            string[] argsStrings = actionArgs.Select(arg => arg?.ToString() ?? "null").ToArray();
            return $"{actionName}({string.Join(", ", argsStrings)})";
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
                if (!p.Evaluate())
                {
                    Debug.Log($"Predicate {p} is not true");
                    Failure = FailureReason.PreconditionsInvalid;
                    return false;
                }
            }
            return true;
        }

        // Add a constraint to this action
        public void AddConstraint(Action<List<Pointer>> constraint)
        {
            constraint?.Invoke(actionArgs); // Apply immediately
        }

        private void ApplyConstraints()
        {
            foreach (var constraint in constraints)
                constraint(actionArgs);
        }

        // Check all constraints for this action
        //public bool SatisfiesConstraints()
        //{
        //    foreach (Func<List<Pointer>, bool> constraint in constraints)
        //   {
        //       if (!constraint(actionArgs))
        //           return false;
        //   }
        //    return true;
        // }

        public bool hasNullValues()
        {
            return actionArgs.Any(arg => arg.value == null);
        }


        public virtual IEnumerator Execute()
        {
            if (executable == null)
            {
                Debug.LogWarning($"Action '{actionName}' has no coroutine executable assigned.");
                Status = ActionStatus.Failed;
                Failure = FailureReason.MissingExecutable;
                yield break;
            }

            Debug.Log($"Started action {this}");

            Status = ActionStatus.InProgress;

            List<object> argsValues = actionArgs.ConvertAll(arg => arg.Get());

            // IMPORTANT: let Unity handle nested yields
            yield return executable(argsValues);

            if (Status == ActionStatus.Interrupted)
            {
                Status = ActionStatus.Failed;
                yield break;
            }

            Status = ActionStatus.Completed;
            Debug.Log($"Action {this} completed execution!!");
        }

        public void Cancel(FailureReason r)
        {
            if (Status == ActionStatus.InProgress)
            {
                Status = ActionStatus.Interrupted;
                Failure = r;
            }
        }

        public bool EffectsHold(WorldState observedState)
        {
            foreach (Predicate effect in effects)
            {
                if (!observedState.ContainsAtom(effect))
                {
                    Debug.LogWarning($"Effect not holding: {effect}");
                    Debug.Log($"Observed State: {string.Join(", ", observedState.GetPredicates().Select(p => p.ToString()))}");

                    Failure = FailureReason.EffectsNotHolding;
                    return false;
                }
            }

            return true;
        }

        public void AllDifferent(List<Pointer> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            int n = values.Count;
            for (int i = 0; i < n; i++)
            {
                List<object> itemsToBan = new List<object>();

                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue;

                    Pointer other = values[j];

                    // Always ban the pointer itself
                    itemsToBan.Add(other);

                    // If the other pointer is currently bound, also ban its value
                    if (other.IsBound())
                    {
                        var val = other.Get();
                        if (val != null)
                            itemsToBan.Add(val);
                    }
                }

                // Ban dynamically both pointers and current values
                values[i].Ban(itemsToBan);
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
                clone.preconditions.Add(new Predicate(pre.Name,pre.Condition, clonedArgs, pre.Value ?? false));
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
                clone.effects.Add(new Predicate(eff.Name, eff.Condition, clonedArgs, eff.Value ?? false));
            }

            // Clone constraints
            clone.constraints = new List<Action<List<Pointer>>>(this.constraints);


            // Copy action name
            clone.actionName = this.actionName;

            // Copy duration and executable if needed
            clone.durationEstimate = this.durationEstimate;
            clone.executable = this.executable;

            return clone;
        }




>>>>>>> Stashed changes
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
