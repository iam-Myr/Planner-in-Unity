using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayActionMove : GameplayAction
{
    [SerializeField] private Player debugTarget;
    [SerializeField] private bool debugPerform;


    private void Update()
    {
        //if (debugPerform)
        //{
       //     perform(debugTarget);
      //      debugPerform = false;
       // }

    }


    public override void perform(object[] arg)
    {
        base.perform(arg);

        // Check if arg[0] is of type Player
        Player player = arg[0] as Player;  // Safe cast to Player

        if (player != null) // Ensure it's not null (i.e., arg[0] was successfully cast)
        {
            Transform target = player.transform;  // Get the player's transform
            transform.position = target.position; // Move to the player's position
            Debug.Log($"Moving to {target.position}");

        }
        else
        {
            Debug.LogError("Argument is not a Player instance.");
        }
    }


    protected override List<Func<object[], bool>> get_preconditions()
    {
        List<Func<object[], bool>> allPreconditions = new List<Func<object[], bool>>(base.get_preconditions());
        allPreconditions.Add(precondition_x);
        return allPreconditions;
    }

    private bool precondition_x(object[] arg)
    {
        return true;
    }



}
