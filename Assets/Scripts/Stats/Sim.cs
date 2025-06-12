using UnityEngine;
using System.Collections.Generic;

public class Sim : MonoBehaviour, IObservable
{
    public float hungerMAX;
    public float sleepMAX;
    public float waterMAX;

    public float hunger;
    public float sleep;
    public float water;

    public float degrationRate = 5f;
    public float threshold = 40f;

    void Awake()
    {
        Register();
    }

    void Start()
    {
        hunger = hungerMAX;
        sleep = sleepMAX;
        water = waterMAX;
    }

    // Update is called once per frame
    void Update()
    {
        hunger -= degrationRate * Time.deltaTime;
        sleep -= degrationRate * Time.deltaTime;
        water -= degrationRate * Time.deltaTime;
    }

    public void Sleep()
    {
        sleep = sleepMAX;
    }

    public void Eat()
    {
        hunger = hungerMAX;
    }

    public void Drink()
    {
        water = waterMAX;
    }

    // Predicates that are relevant to Sim. What about the irrelevant ones?
    public List<Predicate> GetState()
    {
        bool evalSleepy = Domain.isSleepy(new List<object> {sleep, threshold});
        bool evalHunger = Domain.isHungry(new List<object> { hunger, threshold });
        bool evalThirst = Domain.isThirsty(new List<object> { water, threshold });

        bool evalSpawn = Domain.isAt(new List<object> { this, Domain.Spawn.Get() });
        bool evalFood = Domain.isAt(new List<object> { this, Domain.Food.Get() });
        bool evalWater = Domain.isAt(new List<object> { this, Domain.Water.Get() });
        bool evalSleep = Domain.isAt(new List<object> { this, Domain.Sleep.Get() });

        return new List<Predicate> {
            new Predicate(Domain.isSleepy, new List<Pointer> { }, evalSleepy),
            new Predicate(Domain.isHungry, new List<Pointer> { }, evalHunger),
            new Predicate(Domain.isThirsty, new List<Pointer> { }, evalThirst),
            new Predicate(Domain.isAt, new List<Pointer> { Domain.Spawn }, evalSpawn),
            new Predicate(Domain.isAt, new List<Pointer> { Domain.Food }, evalFood),
            new Predicate(Domain.isAt, new List<Pointer> { Domain.Water }, evalWater),
            new Predicate(Domain.isAt, new List<Pointer> { Domain.Sleep }, evalSleep)
            };
    }

    public void Register()
    {
        ObservationManager.Register(this);
    }
}
