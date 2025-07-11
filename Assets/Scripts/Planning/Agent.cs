using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using SimWorld;
using System.Collections;

namespace Planning
{
    public abstract class Agent : MonoBehaviour, IObservableHolder
    {
        private Planner planner;
        private List<PlanAction> currentPlan;
        private WorldState currentState;
        private WorldState currentGoal;
        private List<PlanAction> actionList;

        [Header("Planning Parameters")]
        public int MAXSTEPS = 1000000;
        public float observeCooldown = 2f;
        public float actionTimeout = 5f;

        [Header("-------------------------------")]
        [SerializeField] private bool doYouLikePlanning;

        private float observeTimer = 0f;

        // Abstract domain-specific data to be provided by derived classes
        protected abstract List<WorldState> DomainGoals { get; }
        protected abstract List<PlanAction> DomainActions { get; }
        protected abstract List<Pointer> DomainPointers { get; }

        protected virtual void Awake()
        {
            Register();
            SetPredicateConditions();
        }

        public void Register()
        {
            ObservationManager.Register(this);
        }

        public virtual List<Predicate> GetObservablePredicates() => new List<Predicate>();

        protected virtual void Start()
        {
            // Choose initial goal from the domain's goals
            currentGoal = ChooseGoal(DomainGoals);

            // Generate grounded actions from domain pointers
            List<PlanAction> groundedActions = ActionGenerator.GenerateAllGroundedActions(DomainActions, DomainPointers);

            // Initialize the planner with grounded actions
            planner = new Planner(groundedActions);

            // No plan at start
            currentPlan = null;
        }

        protected virtual void Update()
        {
            // If executing a plan, don't replan
            if (currentPlan != null)
                return;

            // Countdown observe timer
            observeTimer -= Time.deltaTime;

            if (observeTimer <= 0f)
            {
                observeTimer = observeCooldown; // Reset cooldown

                // Select a goal to plan for
                currentGoal = ChooseGoal(DomainGoals);

                // Ask planner to generate a plan from current state to goal
                currentPlan = planner.MakePlan(currentGoal, MAXSTEPS);

                if (currentPlan != null && currentPlan.Count > 0)
                {
                    PrintPlan(currentPlan);
                    ExecutePlan(currentPlan);
                }
                else
                {
                    Debug.Log("No plan generated.");
                    currentPlan = null;
                }
            }
        }

        public WorldState ChooseGoal(List<WorldState> list)
        {
            if (list == null || list.Count == 0)
                throw new InvalidOperationException("Cannot choose a goal: the goal list is null or empty.");

            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }


        public void PrintPlan(List<PlanAction> plan)
        {
            if (plan == null || plan.Count == 0)
            {
                Debug.Log("No plan :(");
                return;
            }

            Debug.Log("==== PLAN ====");
            for (int i = 0; i < plan.Count; i++)
            {
                Debug.Log($"{i + 1}. {plan[i]} ");
            }
            Debug.Log($"==== {plan.Count} steps ====");
        }

        public void ExecutePlan(List<PlanAction> plan)
        {
            StartCoroutine(ExecutePlanCoroutine(plan));
        }

        private IEnumerator ExecutePlanCoroutine(List<PlanAction> plan)
        {
            foreach (PlanAction action in plan)
            {
                if (!action.IsValid())
                {
                    Debug.Log($"{action} not valid! Replanning...");
                    currentPlan = null;
                    yield break;
                }

                Coroutine running = StartCoroutine(action.Execute());

                float elapsed = 0f;

                while (action.GetStatus() == PlanAction.ActionStatus.InProgress && elapsed < action.GetEstimatedDuration())
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                if (action.GetStatus() == PlanAction.ActionStatus.InProgress)
                {
                    action.Cancel();
                    StopCoroutine(running);
                    Debug.LogWarning($"Action {action} timed out and was interrupted.");
                    currentPlan = null;
                    yield break;
                }

                if (action.GetStatus() == PlanAction.ActionStatus.Failed ||
                    action.GetStatus() == PlanAction.ActionStatus.Interrupted)
                {
                    Debug.LogWarning($"Action {action} failed or was interrupted.");
                    currentPlan = null;
                    yield break;
                }
            }

            currentPlan = null;
        }


        public abstract void SetPredicateConditions();

    }

}

