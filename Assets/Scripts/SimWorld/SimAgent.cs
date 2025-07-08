using System.Collections.Generic;
using UnityEngine;
using Planning;
using System.Threading.Tasks;

namespace SimWorld
{
    public class SimAgent : Agent
    {
        protected override List<WorldState> DomainGoals => SimDomain.goalList;
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

        public void Sleep(List<object> args)
        {
            sleep = sleepMAX;
        }

        public void Eat(List<object> args)
        {
            hunger = hungerMAX;
        }

        public void Drink(List<object> args)
        {
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

        public void MoveTo(Vector3 destination)
        {
            transform.position = destination;
        }


        public Vector3 GetPosition() => transform.position;

        public float GetSpeed() => moveSpeed;

        /*public void AddConditions()
        {
            SimDomain.isSlep.AddCondition(() => sleep < threshold);
            SimDomain.isAtt.AddCondition((Area ) => area.Contains(transform.position));
        }*/

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


        public void SetPredicates()
        {
            SimDomain.isAt.SetCondition(isAt);
            SimDomain.isSleepy.SetCondition(isSleepy);
            SimDomain.isHungry.SetCondition(isHungry);
            SimDomain.isThirsty.SetCondition(isThirsty);
        }

        public override List<Observable> GetObservables()
        {
            return new List<Observable> {
                // Add keys
                new Observable(isSleepy, new List<Pointer> {}), //hasCondition(sleepy) 
                new Observable(isHungry, new List<Pointer> {}),
                new Observable(isThirsty, new List<Pointer> {}),
                new Observable(isAt, new List<Pointer> {new Pointer(SimDomain.SpawnArea)}),
                new Observable(isAt, new List<Pointer> {new Pointer(SimDomain.FoodArea)}),
                new Observable(isAt, new List<Pointer> {new Pointer(SimDomain.WaterArea)}),
                new Observable(isAt, new List<Pointer> {new Pointer(SimDomain.SleepArea)})
            };
        }

        /*
        public override List<PlanAction> GetAllActionTemplates() {
             return new List<PlanAction>
             {
                new PlanAction( // Sleep
                    new List<Predicate>
                    {
                        new Predicate(isAt, new List<Pointer> {SimDomain.SleepArea}, true), // isAt(sleep)  
                        new Predicate(hasCondition, new List<Pointer> {Conditions.sleepy}, true)
                    },
                    new List<Predicate>
                    {
                        new Predicate(hasCondition, new List<Pointer> {Conditions.sleepy}, false) //not isSleepy
                    },
                    Sleep),

                new PlanAction( // Eat
                    new List<Predicate>
                    {
                        new Predicate(isAt, new List<Pointer> {SimDomain.FoodArea}, true), // isAt(sleep)  
                        new Predicate(hasCondition, new List<Pointer> {Conditions.hungry}, true)
                    },
                    new List<Predicate>
                    {
                        new Predicate(hasCondition, new List<Pointer> {Conditions.hungry}, false) //not isSleepy
                    },
                    Eat),

                new PlanAction( // Drink
                    new List<Predicate>
                    {
                        new Predicate(isAt, new List<Pointer> {SimDomain.WaterArea}, true), // isAt(sleep)  
                        new Predicate(hasCondition, new List<Pointer> {Conditions.thirsty}, true)
                    },
                    new List<Predicate>
                    {
                        new Predicate(hasCondition, new List<Pointer> {Conditions.thirsty}, false) //not isSleepy
                    },
                    Drink)

                new PlanAction( // Move To
                    new List<Predicate>
                    {
                        new Predicate(isAt, new List<Pointer> {}, true), 
                    },
                    new List<Predicate>
                    {
                        new Predicate(isAt, new List<Pointer> {}, true),
                        new Predicate(isAt, new List<Pointer> {}, false) 
                    },
                    MoveTo)

        };
    }
        */

    }
}
