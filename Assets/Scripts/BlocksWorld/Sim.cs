using UnityEngine;


// Maybe will become Observable??
public class Sim : MonoBehaviour
{
    public float hunger;
    public float sleep;
    public float water;

    private float degrationRate = 0.01f;

    // Update is called once per frame
    void Update()
    {
   
        hunger *= degrationRate * Time.deltaTime;
        sleep *= degrationRate * Time.deltaTime;
        water *= degrationRate * Time.deltaTime;

        if (sleep <= 5) Debug.Log("Sleepy!");
    }
}
