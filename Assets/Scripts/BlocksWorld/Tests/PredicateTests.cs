using NUnit.Framework;
using System;
using System.Collections.Generic;

public class PredicateTests
{
    // Sample predicate function to test with
    private bool isOnTable(object[] args)
    {
        return args.Length == 1 && args[0]?.ToString() == "BlockA";
    }

    [Test]
    public void Predicate_IsSame_ReturnsTrueForEqualPredicates()
    {
        var p1 = new Pointer(); p1.Set("A");
        var pred1 = new Predicate(PredicateLibrary.isClear, new List<Pointer> { p1 });

        var p2 = new Pointer(); p2.Set("A");
        var pred2 = new Predicate(PredicateLibrary.isClear, new List<Pointer> { p2 });

        Assert.IsTrue(pred1.isSame(pred2));
    }

    [Test]
    public void Predicate_IsSame_ReturnsFalseForDifferentFunctions()
    {
        var p = new Pointer(); p.Set("A");

        var pred1 = new Predicate((args) => true, new List<Pointer> { p });
        var pred2 = new Predicate((args) => false, new List<Pointer> { p });

        Assert.IsFalse(pred1.isSame(pred2));
    }

    [Test]
    public void Predicate_IsInstantiated_WorksCorrectly()
    {
        var p1 = new Pointer(); p1.Set("BlockA");
        var p2 = new Pointer();

        var pred = new Predicate(isOnTable, new List<Pointer> { p1 });
        var pred2 = new Predicate(isOnTable, new List<Pointer> { p2 });

        Assert.IsTrue(pred.IsInstantiated());
        Assert.IsFalse(pred2.IsInstantiated());
    }

    [Test]
    public void Predicate_ToString_FormatsCorrectly()
    {
        var p = new Pointer(); p.Set("BlockA");
        var pred = new Predicate(isOnTable, new List<Pointer> { p });

        string str = pred.ToString();
        Assert.IsTrue(str.Contains("isOnTable") && str.Contains("BlockA"));
    }
}
