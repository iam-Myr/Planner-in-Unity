using UnityEngine;

public abstract class PlanObject : MonoBehaviour
{

    public override string ToString()
    {
        return gameObject.name;
    }
}
