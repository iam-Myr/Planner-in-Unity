using UnityEngine;
using System.Collections.Generic;
using Planning;
using SimWorld;

namespace BlocksWorld
{
    public class BlockAgent : Agent
    {
        protected override List<WorldState> DomainGoals => BlockDomain.goalList;
        protected override List<Pointer> DomainPointers => BlockDomain.AllPointers;
   
        public List<Predicate> GetState()
        {
            return BlockDomain.InitialState;
        }

        public override List<PlanAction> GetAllActionTemplates()
        {
            return new List<PlanAction>
            {
                
            };
        }
    }
}
