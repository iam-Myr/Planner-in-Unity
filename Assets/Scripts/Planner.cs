using System;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour
{
    public Player player;
    List<GameplayAction> availableActions;
    internal void initialize(List<GameplayAction> availableActions)
    {
        this.availableActions = new List<GameplayAction> (availableActions);
        Debug.Log("Planner Initialized");
    }
    public List<Step> makePlan()
    {

        // Use search here.
        //
        // Here is Dummy Plan:
        return new List<Step>()
        {
           new Step(availableActions[0], new object[]{player}), // 0 is Move, 1 is Attack
           new Step(availableActions[1], new object[]{player}),
           new Step(availableActions[1], new object[]{player})
        };
    }

    public class Step
    {
        public GameplayAction action;
        public object[] args;

        public Step(GameplayAction action, object[] args)
        {
            this.action = action;
            this.args = args;
        }

        public void perform()
        {
            action.perform(args);
        }
    }


}


