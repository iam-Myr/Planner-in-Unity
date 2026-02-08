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
        private LiftedPlanner planner;
        private List<PlanAction> currentPlan;
        private WorldState currentState;
        private WorldState currentGoal;
        private List<PlanAction> actionList;

        [Header("Planning Parameters")]
        public int MAXSTEPS = 1000000;
        public float observeCooldown = 2f;
        public float actionTimeoutWindow = 3f;
        public bool debug;

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
            //List<PlanAction> groundedActions = ActionGenerator.GenerateAllGroundedActions(DomainActions, DomainPointers);

            //foreach(PlanAction action in groundedActions) 
            //    Debug.Log($"{action}");
            

            // Initialize the planner with lifted actions
            planner = new LiftedPlanner(DomainActions, DomainPointers, debug);
            //planner = new GroundPlanner(groundedActions);

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

                // Get current world state from observation manager
                WorldState initState = ObservationManager.Observe();
                if (debug)
                {
                    Debug.Log("THE INIT");
                    initState.Print();
                }

                // Ask planner to generate a plan from current state to goal
                currentPlan = planner.MakePlan(initState, currentGoal, MAXSTEPS);

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
            // Go through each action in the planned sequence
            foreach (PlanAction action in plan)
            {
                // Check if the action is still valid (its preconditions hold)
                if (!action.IsValid())
                {
                    Debug.Log($"{action} not valid! Replanning...");
                    currentPlan = null; 
                    yield break; 
                }

                // Start executing the action as a coroutine and store the handle
                Coroutine running = StartCoroutine(action.Execute());

                // Track how much time has passed since the action started
                float elapsed = 0f;

                // Wait until the action finishes or the timeout limit is reached
                while (action.GetStatus() == PlanAction.ActionStatus.InProgress &&
                       elapsed < action.GetEstimatedDuration() + actionTimeoutWindow)
                {
                    elapsed += Time.deltaTime; // Increment elapsed time by time since last frame
                    yield return null; // Wait for the next frame
                }

                // If the action is still running after the timeout, interrupt it
                if (action.GetStatus() == PlanAction.ActionStatus.InProgress)
                {
                    action.Cancel(PlanAction.FailureReason.Timeout); // Mark the action as interrupted
                    StopCoroutine(running); // Forcefully stop the coroutine
                    Debug.Log($"Action {action} timed out and was interrupted.");

                    currentPlan = null; 
                    yield break; 
                }

                // Action finished executing
                if (action.GetStatus() == PlanAction.ActionStatus.Completed)
                {
                    WorldState observed = ObservationManager.Observe();

                    //Debug.Log("Expected: ");
                    //foreach (Predicate e in action.GetEffects()) Debug.Log($"  {e}");

                    //Debug.Log("Observed: ");
                    //observed.Print();


                    if (!action.EffectsHold(observed))
                    {
                        Debug.Log($"Action {action} executed but effects were not applied.");
                        currentPlan = null;
                        yield break;
                    }
                }

            }

            // All actions completed successfully — clear the current plan
            currentPlan = null;
        }

    }

}

