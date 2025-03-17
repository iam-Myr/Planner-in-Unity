using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class GameplayAction : MonoBehaviour
{
    [SerializeField] private GameplayActionEnum actionName;

    private List<Func<object[], bool>> preconditions = new List<Func<object[], bool>>();
    private List<Func<object[], bool>> effects = new List<Func<object[], bool>>();


    private void Start()
    {
        preconditions.AddRange(get_preconditions());
        effects.AddRange(get_effects());
    }

    protected virtual List<Func<object[], bool>> get_preconditions() { 
        return new List<Func<object[], bool>> { 
            is_inrange
        };
    }

    protected virtual List<Func<object[], bool>> get_effects()
    {
        return new List<Func<object[], bool>> {
            is_inrange
        };
    }

    private bool is_inrange(object[] arg) //is in range of Player for now
    {
        float threshold = 3f;

        GameObject target = arg[0] as GameObject;
        float dist = Vector2.Distance(transform.position, target.transform.position);

        return dist < threshold;
    }

    public virtual void perform(object[] arg)
    {
        print($"Performing action {actionName}");
    }


}
