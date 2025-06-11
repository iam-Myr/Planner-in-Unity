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

    private float degrationRate = 5f;
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

    public List<Predicate> GetState()
    {
        bool evalSleep = Domain.isSleepy(new List<object> {sleep, threshold});
        bool evalHunger = Domain.isHungry(new List<object> { hunger, threshold });
        bool evalThirst = Domain.isThirsty(new List<object> { water, threshold });


        return new List<Predicate> {
            new Predicate(Domain.isSleepy, new List<Pointer> { }, evalSleep),
            new Predicate(Domain.isHungry, new List<Pointer> { }, evalHunger),
            new Predicate(Domain.isThirsty, new List<Pointer> { }, evalThirst)
            };
    }

    public void Register()
    {
        ObservationManager.Register(this);
    }
}
