using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;


public class Agent : MonoBehaviour
{
    internal IEnumerator doAction(GameplayAction action)
    {
        //action.performAction();
        yield break;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

}

