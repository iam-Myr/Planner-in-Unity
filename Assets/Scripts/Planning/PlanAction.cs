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
                    if (effect.IsOpposite(goal))
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

        public void BanThreats(List<Predicate> goalList)
        {
            foreach(Predicate e in effects)
            {
                e.ApplyBan(goalList);
            }
        }



        public virtual IEnumerator Execute()
        {
            if (executable == null)
            {
                Debug.Log($"Action '{actionName}' has no coroutine executable assigned.");
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


        public PlanAction Clone(Dictionary<Pointer, Pointer> map)
        {
            PlanAction clone = (PlanAction)Activator.CreateInstance(GetType());

            clone.actionName = actionName;
            clone.durationEstimate = durationEstimate;

            // Clone args
            clone.actionArgs = actionArgs
                .Select(arg => arg.Clone(map))
                .ToList();

            // Clone preconditions
            clone.GetPreconditions().Clear();
            foreach (var p in GetPreconditions())
            {
                clone.GetPreconditions().Add(p.Clone(map));
            }

            // Clone effects
            clone.GetEffects().Clear();
            foreach (var e in GetEffects())
            {
                clone.GetEffects().Add(e.Clone(map));
            }

            clone.AllDifferent(clone.actionArgs);

            clone.executable = this.executable;

            return clone;
        }

        public override bool Equals(object obj)
        {
            if (obj is not PlanAction other)
                return false;

            if (actionName != other.actionName)
                return false;

            if (actionArgs.Count != other.actionArgs.Count)
                return false;

            for (int i = 0; i < actionArgs.Count; i++)
            {
                var a = actionArgs[i].Get();
                var b = other.actionArgs[i].Get();

                if (!Equals(a, b))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = actionName.GetHashCode();

            foreach (var arg in actionArgs)
            {
                var v = arg.Get();
                hash = hash * 31 + (v?.GetHashCode() ?? 0);
            }

            return hash;
        }

    }


 public class ActionInit : PlanAction
        {
        public ActionInit() { }
        public ActionInit(List<Predicate> initEffects)
        {
            actionName = "Init";
            // Manually populate base effects list
            effects = initEffects;
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