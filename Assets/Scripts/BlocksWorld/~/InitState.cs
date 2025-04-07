using System;
using System.Collections.Generic;
using UnityEngine;

public class InitState : WorldState
{
    private SharedVar A = new SharedVar();
    private SharedVar B = new SharedVar();
    private SharedVar C = new SharedVar();

    public InitState()
    {
        A.value = 
        B.value = 
        C.value = 

        atomArgs = new SharedVar[] { A, B, C };

        atoms = new List<(Func<object[], bool>, object[])>
        {
            (PredicateLibrary.isClear, new object[] {C}),
            (PredicateLibrary.isClear, new object[] {B}),
            (PredicateLibrary.isOn, new object[] {B, A}),
        };
    }
   
}
