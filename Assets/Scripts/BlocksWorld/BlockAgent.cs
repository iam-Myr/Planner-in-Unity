using UnityEngine;
using System.Collections.Generic;
using Planning;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Rendering;


namespace BlocksWorld
{
    public class BlockAgent : Agent
    {
        Block holdingBlock;

        protected override List<WorldState> DomainGoals => BlockDomain.GetGoals();
        protected override List<PlanAction> DomainActions => new List<PlanAction>
        {
            new ActionDrop().AddExecutable(Drop, 1f),
            new ActionPickup().AddExecutable(Pickup, 1f),
            new ActionMove().AddExecutable(Move, 1f),
        };

        protected override List<Pointer> DomainPointers => BlockDomain.AllPointers;

        void Awake()
        {
            base.Awake();
        }

        public bool isHolding(Block block)
        {
   
            return holdingBlock == block;
        }

        public bool isHandEmpty()
        {
            return holdingBlock == null;
        }


        public override List<Predicate> GetObservablePredicates()
        {
            List<Predicate> observables = new List<Predicate>();

            // Add basic agent stats
            observables.Add(new Predicate(BlockDomain.isHandEmpty.TheFunc, new List<Pointer>()));

            // Add isAt predicates for all area-type pointers in the domain
            foreach (Pointer p in BlockDomain.AllPointers)
            {
                // Only include PlanObjects that are areas
                if (p.value is Block)
                {
                    observables.Add(new Predicate(BlockDomain.isHolding.TheFunc, new List<Pointer> { new Pointer(p.value) }));
                }
            }

            return observables;
        }

        // ACTIONS
        public IEnumerator Move(List<object> args)
        {
            Debug.Log("Moving!...");

            Block current = (Block)args[0];
            Block to = (Block)args[1];
            Block from = (Block)args[2];

            Vector3 dest = to.GetPosition() + Vector3.up;
            yield return current.MoveToAsync(dest);
        }

        public IEnumerator Pickup(List<object> args)
        {
            Debug.Log("Pickup!...");

            Block current = (Block)args[0];
            Block from = (Block)args[1];


            Vector3 dest = current.GetPosition() + Vector3.up * 1.1f;
            yield return current.MoveToAsync(dest);
        }

        public IEnumerator Drop(List<object> args)
        {
            Debug.Log("Drop!...");

            Block current = (Block)args[0];
            Block to = (Block)args[1];


            Vector3 dest = to.GetPosition() + Vector3.up * 1.1f;
            yield return current.MoveToAsync(dest);
        }
    }
}
