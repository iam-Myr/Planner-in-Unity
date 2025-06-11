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

    public List<Predicate> GetState()
    {
        bool eval = Domain.isSleepy(new List<object> {sleep, threshold});


        return new List<Predicate> {
            new Predicate(Domain.isSleepy, new List<Pointer> { }, eval)
            };
    }

    public void Register()
    {
        ObservationManager.Register(this);
    }
}
