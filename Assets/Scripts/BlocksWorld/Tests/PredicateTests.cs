using NUnit.Framework;
using System;
using System.Collections.Generic;

public class PredicateTests
{
    // Helper to create a Predicate with simple func name and args
    private Predicate CreatePredicate(string funcName, params Pointer[] args)
    {
        Func<object[], bool> dummyFunc = _ => true; // dummy function
        // Override func.Method.Name with reflection is complex, so let's simulate with a wrapper class or just use dummyFunc.

        var pred = new Predicate(dummyFunc, new List<Pointer>(args), true);

        // Ideally, you'd want to set func.Method.Name, but this is a readonly property.
        // Instead, you could differentiate predicates by some other means in your real Equals implementation.
        // For this test, we assume func is the same dummyFunc for all predicates.

        return pred;
    }
    // Dummy function shared among predicates
    static Func<object[], bool> dummyFunc = args => true;

    [Test]
    public void Contains_ShouldFindEqualPredicate()
    {
        Func<object[], bool> dummyFunc = args => true;

        var pointer1 = new Pointer("A");
        var pointer2 = new Pointer("B");

        var pred1 = new Predicate(dummyFunc, new List<Pointer> { pointer1, pointer2 }, true);
        var pred2 = new Predicate(dummyFunc, new List<Pointer> { pointer1.Clone(), pointer2.Clone() }, true);

        var list = new List<Predicate> { pred1 };

        Assert.IsTrue(list.Contains(pred2)); // Should pass now
    }


    [Test]
    public void Contains_ShouldNotFindDifferentPredicate()
    {
        var pointer1 = new Pointer("A");
        var pointer2 = new Pointer("B");
        var pointer3 = new Pointer("C");

        var pred1 = CreatePredicate("func1", pointer1, pointer2);
        var pred2 = CreatePredicate("func1", pointer1, pointer3); // different args

        var list = new List<Predicate> { pred1 };

        Assert.IsFalse(list.Contains(pred2), "List should not contain a different predicate");
    }
    [Test]
    public void Contains_ShouldFindEqualPredicate_InListOfMany()
    {
        // Setup pointers
        var p1 = new Pointer("X");
        var p2 = new Pointer("Y");
        var p3 = new Pointer("Z");

        // Create several predicates with different args and negation flags
        var predA = new Predicate(dummyFunc, new List<Pointer> { p1, p2 }, true);
        var predB = new Predicate(dummyFunc, new List<Pointer> { p2, p3 }, false);
        var predC = new Predicate(dummyFunc, new List<Pointer> { p1, p3 }, true);

        // Add them to list
        var list = new List<Predicate> { predA, predB, predC };

        // Create a new predicate equal to predA (clone args, same func, same negation)
        var predToFind = new Predicate(dummyFunc, new List<Pointer> { p1.Clone(), p2.Clone() }, true);

        // Check contains
        bool contains = list.Contains(predToFind);

        Assert.IsTrue(contains, "List should contain an equal Predicate to predToFind");
    }
}
