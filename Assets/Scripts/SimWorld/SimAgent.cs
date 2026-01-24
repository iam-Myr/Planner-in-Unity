using System.Collections.Generic;
using UnityEngine;
using Planning;
using System.Threading.Tasks;
using System.Collections;
using Mono.Cecil;
using Unity.VisualScripting;

namespace SimWorld
{
    public class SimAgent : Agent
    {
        protected override List<WorldState> DomainGoals => SimDomain.GetGoals();
        protected override List<PlanAction> DomainActions => new List<PlanAction>
        {
            new ActionSleep().AddExecutable(Sleep, 1f),
            new ActionEat().AddExecutable(Eat, 1f),
            new ActionDrink().AddExecutable(Drink, 1f),
            new ActionMoveTo().AddExecutable(MoveTo, 5f) 
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
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private AudioSource audioData;

        public List<AudioClip> audioClips = new List<AudioClip>(); //0 - WATER, 1- FOOD, 2-SLEEP

        void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioData = GetComponent<AudioSource>();
        }

        void Start()
        {
            base.Start();

            hunger = hungerMAX;
            sleep = sleepMAX;
            water = waterMAX;
            //water = 10;
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();

            hunger -= degrationRate * Time.deltaTime;
            sleep -= degrationRate * Time.deltaTime;
            water -= degrationRate * Time.deltaTime;


            // Debugging hehe
            if (Input.GetMouseButtonDown(0)) 
            {
                water = waterMAX;
            }

            if (Input.GetMouseButtonDown(1))
            {
                Area spawn = (Area)SimDomain.SpawnArea.Get();
                transform.position = spawn.GetPosition();
            }
        }

        public IEnumerator Sleep(List<object> args)
        {
            Debug.Log("Sleeping...");
            audioData.clip = audioClips[2];
            audioData.Play();

            yield return new WaitForSeconds(1f); // Optional delay
            sleep = sleepMAX;
        }

        public IEnumerator Eat(List<object> args)
        {
            Debug.Log("Eating!...");
            audioData.clip = audioClips[1];
            audioData.Play();
            yield return new WaitForSeconds(1f);
            hunger = hungerMAX;
        }

        public IEnumerator Drink(List<object> args)
        {
            Debug.Log("Drinking!...");
            audioData.clip = audioClips[0];
            audioData.Play();
            yield return new WaitForSeconds(1f);
            water = waterMAX;
        }

        public IEnumerator MoveTo(List<object> args)
        {
            Debug.Log("Moving...");
            animator.SetBool("isMoving", true);
            Area area = (Area)args[0]; 
            Vector3 destination = area.GetPosition(); 
            
            
            while (Vector3.Distance(transform.position, destination) > 0.1f)
            {

                // Determine direction relative to current position
                float direction = destination.x - transform.position.x;

                spriteRenderer.flipX = !(direction > 0); // facing right
            

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    destination,
                    moveSpeed * Time.deltaTime
                );
                yield return null; // wait for next frame
            }
            animator.SetBool("isMoving", false);
        }

        public bool isSleepy(List<object> args)
        {
            return sleep < threshold;
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
            if (args.Count == 0 || !SimDomain.IsOfType<Area>(args[0]))
                return false;

            Area a = (Area)args[0];
            return a.Contains(transform.position);
        }


        public override void SetPredicateConditions()
        {
            SimDomain.isAt.SetCondition(isAt);
            SimDomain.isSleepy.SetCondition(isSleepy);
            SimDomain.isHungry.SetCondition(isHungry);
            SimDomain.isThirsty.SetCondition(isThirsty);
        }

        public override List<Predicate> GetObservablePredicates()
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

        /*
         * public float GetValue(Conditions condition)
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
        
          public enum Conditions { sleepy = 0, hungry, thirsty }

        public bool hasCondition(List<object> args)
        {
            Conditions condition = (Conditions)args[0];
            return GetValue(condition) < GetThreshold(condition);
        }
         
         */



    }
}
