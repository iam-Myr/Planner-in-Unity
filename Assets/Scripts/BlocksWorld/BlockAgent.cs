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
            //new ActionMove().AddExecutable(Move, 1f),
            new ActionDrop().AddExecutable(Drop, 2f),
           new ActionPickup().AddExecutable(Pickup, 2f)
            
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
            observables.Add(BlockDomain.isHandEmpty.Instantiate(new List<Pointer>(), null));
            //observables.Add(new Predicate(BlockDomain.isHandEmpty.Name, BlockDomain.isHandEmpty.Condition, new List<Pointer>()));

            // Add isAt predicates for all area-type pointers in the domain
            foreach (Pointer p in BlockDomain.AllPointers)
            {
                // Only include PlanObjects that are areas
                if (p.value is Block)
                {
                    observables.Add(BlockDomain.isHolding.Instantiate(new List<Pointer> { new Pointer(p.value) }, null));
                    //observables.Add(new Predicate(BlockDomain.isHolding.Name, BlockDomain.isHolding.Condition, new List<Pointer> { new Pointer(p.value) }));
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
            Block current = (Block)args[0];
            Block from = (Block)args[1];

            Debug.Log($"Pickup!... {current} from {from}");
            yield return new WaitForSeconds(1f);

            holdingBlock = current;

            Vector3 dest = current.GetPosition() + Vector3.up * 1.1f;
            yield return current.MoveToAsync(dest);
        }

        public IEnumerator Drop(List<object> args)
        {

            Block current = (Block)args[0];
            Block to = (Block)args[1];

            Debug.Log($"Drop!...{current} to {to}");
            yield return new WaitForSeconds(1f);

            holdingBlock = null; //normally don't do this with logic cause if action fails, this remains

            Vector3 dest = to.GetPosition() + Vector3.up;
            yield return current.MoveToAsync(Vector3.zero);
        }
    }
}
