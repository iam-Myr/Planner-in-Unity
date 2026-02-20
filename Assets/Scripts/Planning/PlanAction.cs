using BlocksWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

namespace Planning
{
    public abstract class PlanAction
    {
        public string actionName;
        public List<Pointer> actionArgs = new List<Pointer>();
        protected List<Predicate> preconditions = new List<Predicate>();
        protected List<Predicate> effects = new List<Predicate>();
        protected Func<List<object>, IEnumerator> executable;
        public float durationEstimate { get; protected set; } = 20f; // fallback

        private List<Func<List<Pointer>, bool>> constraints
        = new List<Func<List<Pointer>, bool>>();

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

        // For each item in the list
        //   - take all other items (items except itself)
        //   - convert them to a List<Pointer>
        //   - immediately call Ban on that list
        public void AllDifferent(List<Pointer> items)
        {
            items.ForEach(item => item.Ban(items.Except(new[] { item }).ToList()));
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
        public void AddConstraint(Func<List<Pointer>, bool> constraint)
        {
            constraints.Add(constraint);
        }

        // Check all constraints for this action
        public bool SatisfiesConstraints()
        {
            foreach (Func<List<Pointer>, bool> constraint in constraints)
            {
                if (!constraint(actionArgs))
                    return false;
            }
            return true;
        }

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
            clone.constraints = new List<Func<List<Pointer>, bool>>(this.constraints);


            // Copy action name
            clone.actionName = this.actionName;

            // Copy duration and executable if needed
            clone.durationEstimate = this.durationEstimate;
            clone.executable = this.executable;

            // Apply AllDifferent to cloned arguments so banned lists are correct
            clone.AllDifferent(clone.actionArgs);

            return clone;
        }




    }


 public class ActionInit : PlanAction
        {

        private List<Predicate> initEffects;
        public ActionInit() { }
        public ActionInit(List<Predicate> initEffects)
        {
            actionName = "Init";
            this.initEffects = initEffects;

            // Manually populate base effects list
            this.effects = initEffects
                .Select(p => new Predicate(p.Name, p.Condition, p.Args.Select(a => a.Clone()).ToList(), p.Value ?? false))
                .ToList();
        }

        public override PlanAction CreateNew(List<Pointer> args) 
            {
                return this; // Init action is a singleton with no arguments
        }

            #region Preconditions
            public override List<Predicate> InitPreconditions()
            {
                return new List<Predicate>{};
            }
        #endregion

        #region Effects
        public override List<Predicate> InitEffects()
        {
            return effects;
        }
        #endregion
    }
    

}