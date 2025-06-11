using UnityEngine;
using UnityEngine.UI;

public class SimUI : MonoBehaviour
{
    public Slider sleepBar;
    public Slider hungerBar;
    public Slider waterBar;

    public Image sleepBarFill;
    public Sim sim;

    void Start()
    {
        sleepBar.maxValue = sim.sleepMAX;
        hungerBar.maxValue = sim.hungerMAX;
        waterBar.maxValue = sim.waterMAX;
    }

    void Update()
    {
        sleepBar.value = sim.sleep;
        hungerBar.value = sim.hunger;
        waterBar.value = sim.water;

        if (sleepBar.value < sim.threshold)
            sleepBarFill.color = Color.red;
        else
            sleepBarFill.color = Color.green;
    }
}
