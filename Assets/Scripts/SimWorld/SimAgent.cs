using System.Collections.Generic;
using UnityEngine;
using Planning;
using System.Threading.Tasks;

namespace SimWorld
{
    public class SimAgent : Agent, IMoveProvider
    {
        protected override List<WorldState> DomainGoals => SimDomain.goalList;
        protected override List<PlanAction> DomainActions => SimDomain.ActionTemplates;
        protected override List<Pointer> DomainPointers => SimDomain.AllPointers;

        // Movement
        public float moveSpeed;

        //Stats
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
            base.Awake();
        }

        void Start()
        {
            base.Start();

            hunger = hungerMAX;
            sleep = sleepMAX;
            water = waterMAX;
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();

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

        public async Task MoveTo(Vector3 destination, float speed)
        {
            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                await Task.Yield();
            }
        }

        public Vector3 GetPosition() => transform.position;

        public float GetSpeed() => moveSpeed;

        // Predicates that are relevant to Sim. What about the irrelevant ones?
        public override List<Predicate> GetState()
        {
            bool evalSleepy = SimDomain.isSleepy(new List<object> { sleep, threshold });
            bool evalHunger = SimDomain.isHungry(new List<object> { hunger, threshold });
            bool evalThirst = SimDomain.isThirsty(new List<object> { water, threshold });

            bool evalSpawn = SimDomain.isAt(new List<object> { this, SimDomain.Spawn.Get() });
            bool evalFood = SimDomain.isAt(new List<object> { this, SimDomain.Food.Get() });
            bool evalWater = SimDomain.isAt(new List<object> { this, SimDomain.Water.Get() });
            bool evalSleep = SimDomain.isAt(new List<object> { this, SimDomain.Sleep.Get() });

            return new List<Predicate> {
            new Predicate(SimDomain.isSleepy, new List<Pointer> { }, evalSleepy),
            new Predicate(SimDomain.isHungry, new List<Pointer> { }, evalHunger),
            new Predicate(SimDomain.isThirsty, new List<Pointer> { }, evalThirst),
            new Predicate(SimDomain.isAt, new List<Pointer> { SimDomain.Spawn }, evalSpawn),
            new Predicate(SimDomain.isAt, new List<Pointer> { SimDomain.Food }, evalFood),
            new Predicate(SimDomain.isAt, new List<Pointer> { SimDomain.Water }, evalWater),
            new Predicate(SimDomain.isAt, new List<Pointer> { SimDomain.Sleep }, evalSleep)
            };
        }
    }
}
