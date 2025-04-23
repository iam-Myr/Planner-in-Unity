using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ActionTests
{
    private class TestAction : Action
    {
        public TestAction()
        {
            actionName = "TestAction";

            var x = new Pointer();
            var y = new Pointer();

            actionArgs = new List<Pointer> { x, y };

            preconditions = new List<Predicate>
            {
                new Predicate((args) => true, new List<Pointer>{ x })
            };

            effects = new List<Predicate>
            {
                new Predicate((args) => true, new List<Pointer>{ x, y })
            };
        }
    }

    [Test]
    public void Action_Clone_MaintainsStructureAndValue()
    {
        var action = new TestAction();
        action.actionArgs[0].Set("BlockA");
        action.actionArgs[1].Set("BlockB");

        var clone = action.Clone();

        Assert.AreEqual(action.Print(), clone.Print());
        Assert.AreEqual(action.GetPreconditions().Count, clone.GetPreconditions().Count);
        Assert.AreEqual(action.GetEffects().Count, clone.GetEffects().Count);

        // Ensure the clone's pointers are separate instances
        Assert.AreNotSame(action.actionArgs[0], clone.actionArgs[0]);
        Assert.AreEqual(action.actionArgs[0].Get(), clone.actionArgs[0].Get());
    }

    [Test]
    public async Task Action_Execute_DoesNotThrow()
    {
        var action = new TestAction();
        Assert.DoesNotThrowAsync(async () => await action.Execute());
    }
}
