using NUnit.Framework;
using System;

public class PointerTests
{
    [Test]
    public void Pointer_BindsToValue_ResolvesCorrectly()
    {
        var p = new Pointer();
        p.Set("Hello");

        Assert.AreEqual("Hello", p.Get());
        Assert.IsTrue(p.IsBound());
        Assert.IsFalse(p.IsPointer());
    }

    [Test]
    public void Pointer_BindsToPointer_ResolvesCorrectly()
    {
        var p1 = new Pointer();
        var p2 = new Pointer();

        p1.BindTo(p2);
        p2.Set(42);

        Assert.AreEqual(42, p1.Get());
        Assert.AreEqual(42, p2.Get());
        Assert.IsTrue(p1.IsBound());
        Assert.IsTrue(p1.IsPointer());
    }

    [Test]
    public void Pointer_BindTwoPointersWithSameValue_NoConflict()
    {
        var p1 = new Pointer();
        var p2 = new Pointer();

        p1.Set("Value");
        p2.Set("Value");

        p1.BindTo(p2); // Should not throw

        Assert.AreEqual(p1.Get(), p2.Get());
    }

    [Test]
    public void Pointer_BindTwoPointersWithConflictingValues_Throws()
    {
        var p1 = new Pointer();
        var p2 = new Pointer();

        p1.Set("A");
        p2.Set("B");

        Assert.Throws<InvalidOperationException>(() => p1.BindTo(p2));
    }

    [Test]
    public void Pointer_CycleDetectionOnBinding_Throws()
    {
        var p1 = new Pointer();
        var p2 = new Pointer();
        var p3 = new Pointer();

        p1.BindTo(p2);
        p2.BindTo(p3);

        Assert.Throws<InvalidOperationException>(() => p3.BindTo(p1)); // Would create a cycle
    }

    [Test]
    public void Pointer_SetValueThroughChain_ResolvesEverywhere()
    {
        var a = new Pointer();
        var b = new Pointer();
        var c = new Pointer();

        a.BindTo(b);
        b.BindTo(c);

        c.Set("Chain!");

        Assert.AreEqual("Chain!", a.Get());
        Assert.AreEqual("Chain!", b.Get());
        Assert.AreEqual("Chain!", c.Get());
    }
}
