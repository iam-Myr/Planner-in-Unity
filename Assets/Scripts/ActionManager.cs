using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private Planner planner;
    private Agent agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        agent = GetComponentInChildren<Agent>();
        planner = GetComponentInChildren<Planner>();

        planner.initialize(agent.GetComponents<GameplayAction>().ToList());
        while (true)
        {
            List<Planner.Step> plan = planner.makePlan();
            foreach (Planner.Step step in plan) {
                //validate action

                //perform action
                step.perform();
            }
            yield return new WaitForSeconds(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
