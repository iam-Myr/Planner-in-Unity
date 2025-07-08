using System.Collections.Generic;
using UnityEngine;
using Planning;
using System.Threading.Tasks;

namespace SimWorld
{
    public class SimAgent : Agent
    {
        protected override List<WorldState> DomainGoals => SimDomain.GetGoals();
        protected override List<PlanAction> DomainActions => new List<PlanAction>
        {   
            new ActionSleep().AddExecutable(Sleep),
            new ActionEat().AddExecutable(Eat),
            new ActionDrink().AddExecutable(Drink),
            new ActionMoveTo().AddExecutable(MoveTo)
        };

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
            SetPredicateConditions();
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

        public void Sleep(List<object> args)
        {
            Debug.Log("Sleeping!...");
            sleep = sleepMAX;
        }

        public void Eat(List<object> args)
        {
            Debug.Log("Eating!...");
            hunger = hungerMAX;
        }

        public void Drink(List<object> args)
        {
            Debug.Log("Drinking!...");
            water = waterMAX;
        }

        /*public async Task MoveTo(Vector3 destination)
        {
            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                await Task.Yield();
            }
        }*/

        public void MoveTo(List<object> args)
        {
            Debug.Log("Moving...");
            Area area = (Area)args[0]; 
            Vector3 destination = area.GetPosition(); 
            transform.position = destination;
        }

        public Vector3 GetPosition() => transform.position;

        public float GetSpeed() => moveSpeed;

        public bool isSleepy(List<object> args)
        {
            return sleep < threshold;
        }

        public enum Conditions { sleepy = 0, hungry, thirsty }

        public bool hasCondition(List<object> args)
        {
            Conditions condition = (Conditions)args[0];
            return GetValue(condition) < GetThreshold(condition);
        }

        public bool isThirsty(List<object> args)
        {
            return water < threshold;
        }

        public bool isHungry(List<object> args)
        {
            return hunger < threshold;
        }


        public bool isAt(List<object> args)
        {
            Area a = (Area)args[0];
            return a.Contains(transform.position);
        }

        public float GetValue(Conditions condition)
        {
            switch (condition)
            {
                case Conditions.sleepy:
                    return sleep;
                case Conditions.hungry:
                    return hunger;
                case Conditions.thirsty:
                    return water;
                default:
                    throw new System.NotImplementedException();
            }
        }

        public float GetThreshold(Conditions condition)
        {
            return threshold;
        }


        public void SetPredicateConditions()
        {
            SimDomain.isAt.SetCondition(isAt);
            SimDomain.isSleepy.SetCondition(isSleepy);
            SimDomain.isHungry.SetCondition(isHungry);
            SimDomain.isThirsty.SetCondition(isThirsty);
        }

        public override List<Predicate> GetPredicates()
        {
            return new List<Predicate> {
                new Predicate(isSleepy, new List<Pointer>()),
                new Predicate(isHungry, new List<Pointer>()),
                new Predicate(isThirsty, new List<Pointer>()),
                new Predicate(isAt, new List<Pointer> { new Pointer(SimDomain.SpawnArea) }),
                new Predicate(isAt, new List<Pointer> { new Pointer(SimDomain.FoodArea) }),
                new Predicate(isAt, new List<Pointer> { new Pointer(SimDomain.WaterArea) }),
                new Predicate(isAt, new List<Pointer> { new Pointer(SimDomain.SleepArea) }),
            };
        }

    }
}
