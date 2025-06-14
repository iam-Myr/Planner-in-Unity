using UnityEngine;
using System.Collections.Generic;

public class BlockAgent : Agent
{
    protected override List<WorldState> DomainGoals => BlockDomain.goalList;
    protected override List<PlanAction> DomainActions => BlockDomain.ActionTemplates;
    protected override List<Pointer> DomainPointers => BlockDomain.AllPointers;

    public override List<Predicate> GetState()
    {
        return BlockDomain.InitialState;
    }
}
