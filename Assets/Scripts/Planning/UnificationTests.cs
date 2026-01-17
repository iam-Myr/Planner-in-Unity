using UnityEngine;
using System.Collections.Generic;
using Planning;

public class UnificationTester : MonoBehaviour
{
    void Start()
    {
        RunTests();
    }

    void RunTests()
    {
        // 1️⃣ Simple pointer to value
        var x = new Pointer(typeof(string));
        bool test1 = Unification.Unify(x, "BlockA");
        Debug.Log($"Test1: {test1} (x={x.Get()})"); // true, x = "BlockA"

        // 2️⃣ Pointer to pointer
        var y = new Pointer(typeof(string));
        var z = new Pointer(typeof(string));
        bool test2 = Unification.Unify(y, z);
        Debug.Log($"Test2: {test2} (y bound? {y.IsBound()}, z bound? {z.IsBound()})"); // true
        Unification.Unify(z, "BlockB");
        Debug.Log($"After binding z, y={y.Get()}, z={z.Get()}"); // y = BlockB, z = BlockB

        // 3️⃣ Predicate with concrete arguments
        var pred1 = new Predicate(null, new List<Pointer> { new Pointer("A"), new Pointer("B") });
        var pred2 = new Predicate(null, new List<Pointer> { new Pointer("A"), new Pointer("B") });
        bool test3 = Unification.Unify(pred1, pred2);
        Debug.Log($"Test3: {test3}"); // true

        // 4️⃣ Predicate with variables
        var A = new Pointer(typeof(string));
        var B = new Pointer(typeof(string));
        var pred3 = new Predicate(null, new List<Pointer> { A, B });
        var pred4 = new Predicate(null, new List<Pointer> { new Pointer("Block1"), new Pointer("Block2") });
        bool test4 = Unification.Unify(pred3, pred4);
        Debug.Log($"Test4: {test4} (A={A.Get()}, B={B.Get()})"); // true, A=Block1, B=Block2

        // 5️⃣ Predicate unification failure
        var pred5 = new Predicate(null, new List<Pointer> { new Pointer("X") });
        var pred6 = new Predicate(null, new List<Pointer> { new Pointer("Y") });
        bool test5 = Unification.Unify(pred5, pred6);
        Debug.Log($"Test5: {test5}"); // false

        // 6️⃣ Nested predicates
        var inner1 = new Predicate(null, new List<Pointer> { new Pointer("C") });
        var inner2 = new Predicate(null, new List<Pointer> { new Pointer("C") });
        var outer1 = new Predicate(null, new List<Pointer> { new Pointer(inner1) });
        var outer2 = new Predicate(null, new List<Pointer> { new Pointer(inner2) });
        bool test6 = Unification.Unify(outer1, outer2);
        Debug.Log($"Test6: {test6}"); // true
    }
}
