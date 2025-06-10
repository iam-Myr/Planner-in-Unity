using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class Agent : MonoBehaviour, IObservable
{
    private Planner planner;
    private List<Action> currentPlan;
    private WorldState currentState;
    private WorldState currentGoal;
    private List<Action> actionList;

    public const int MAXSTEPS = 1000000;

    // Movement
    public float moveSpeed;

    // Observation cooldown (seconds)
    public float observeCooldown = 2f;
    private float observeTimer = 0f;

    void Awake()
    {
        Register();
    }

    void Start()
    {
        // Load Goal
        currentGoal = ChooseGoal(Problem.goalList);

        // Load actions
        actionList = Domain.ActionTemplates;

        // Ground actions
        List<Action> groundedActions = ActionGenerator.GenerateAllGroundedActions(actionList, Problem.AllPointers);

        // Initialize planner
        planner = new Planner(groundedActions);

        // Start with no current plan
        currentPlan = null;
    }

    void Update()
    {
        // If currently executing a plan, do nothing here
        if (currentPlan != null)
        {
            return;
        }

        // Decrement timer
        observeTimer -= Time.deltaTime;

        if (observeTimer <= 0f)
        {
            observeTimer = observeCooldown;  // reset cooldown timer

            // Observe current world state
            currentState = ObservationManager.Observe();

            // Make plan from current state towards goal
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
        // Chooses first goal for now. Maybe sort?
        return list[0];
    }

    public void PrintPlan(List<Action> plan)
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

    public async void ExecutePlan(List<Action> plan)
    {
        foreach (Action action in plan)
        {
            await action.Execute(this);
        }
        // Plan finished, allow replanning next update
        currentPlan = null;
    }

    public List<Predicate> GetState()
    {
        bool eval = Domain.isAt(new List<object> { this, Problem.Spawn.Get() });

        return new List<Predicate> {
            new Predicate(Domain.isAt, new List<Pointer> { Problem.Spawn }, eval)
        };
    }

    public void Register()
    {
        ObservationManager.Register(this);
    }
}
