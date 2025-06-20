using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public abstract class Agent : MonoBehaviour, IObservable
{
    private Planner planner;
    private List<PlanAction> currentPlan;
    private WorldState currentState;
    private WorldState currentGoal;
    private List<PlanAction> actionList;

    public const int MAXSTEPS = 1000000;

    // Observation cooldown (seconds)
    public float observeCooldown = 2f;
    private float observeTimer = 0f;

    // Abstract domain-specific data to be provided by derived classes
    protected abstract List<WorldState> DomainGoals { get; }
    protected abstract List<PlanAction> DomainActions { get; }
    protected abstract List<object> DomainObjects { get; }

    protected virtual void Awake()
    {
        Register();
    }

    public void Register()
    {
        ObservationManager.Register(this);
    }

    public virtual List<Predicate> GetState() => new List<Predicate>();

    protected virtual void Start()
    {
        // Choose initial goal from the domain's goals
        currentGoal = ChooseGoal(DomainGoals);

        // Load action templates from domain
        actionList = DomainActions;


        // Generate grounded actions from domain pointers
        List<PlanAction> groundedActions = ActionGenerator.GenerateAllGroundedActions(actionList, DomainObjects);

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

            // Get current world state from observation manager
            currentState = ObservationManager.Observe();

            // Select a goal to plan for
            currentGoal = ChooseGoal(DomainGoals);

            // Ask planner to generate a plan from current state to goal
            currentPlan = planner.MakePlan(currentState, currentGoal, MAXSTEPS);

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
            Debug.Log($"{i + 1}. {plan[i].Print()} ");
        }
        Debug.Log($"==== {plan.Count} steps ====");
    }

    public async void ExecutePlan(List<PlanAction> plan)
    {
        foreach (PlanAction action in plan)
        {
           // if (!action.isValid()) return;
            await action.Execute(this);
        }
        // Finished plan; allow replanning next update
        currentPlan = null;
    }
}
