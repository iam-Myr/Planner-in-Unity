using System.Collections.Generic;
using UnityEngine;
using Planning;
using System;

public class UnificationTest : MonoBehaviour
{
    void Start()
    {
        RunTests();
    }

    void RunTests()
    {
        Func<List<object>, bool> dummy = _ => true;

        Pointer x = new Pointer(typeof(object));
        Pointer y = new Pointer(typeof(object));
        Pointer A = new Pointer("A");

        Test("Case 1: x vs y",
            new Planning.Predicate("holding", dummy, new List<Pointer> { x }),
            new Planning.Predicate("holding", dummy, new List<Pointer> { y })
        );

        Test("Case 2: x vs A",
            new Planning.Predicate("holding", dummy, new List<Pointer> { x }),
            new Planning.Predicate("holding", dummy, new List<Pointer> { A })
        );

        Test("Case 3: A vs x",
            new Planning.Predicate("holding", dummy, new List<Pointer> { A }),
            new Planning.Predicate("holding", dummy, new List<Pointer> { x })
        );

        Test("Case 4: A vs A",
            new Planning.Predicate("holding", dummy, new List<Pointer> { A }),
            new Planning.Predicate("holding", dummy, new List<Pointer> { A })
        );
    }

    void Test(string label, Planning.Predicate p1, Planning.Predicate p2)
    {
        Debug.Log($"\n===== {label} =====");

        var result = Unification.TryUnify(p1, p2);

        if (result == null)
        {
            Debug.Log("❌ FAIL");
            return;
        }

        Debug.Log("✅ SUCCESS");

        foreach (var kv in result)
            Debug.Log($"{kv.Key.Name} → {kv.Value}");
    }
}