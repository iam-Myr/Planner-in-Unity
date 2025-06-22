using UnityEngine;
using UnityEngine.UI;

namespace SimWorld
{
    public class SimUI : MonoBehaviour
    {
        public Slider sleepBar;
        public Slider hungerBar;
        public Slider waterBar;

        public Image sleepBarFill;
        public Image hungerBarFill;
        public Image waterBarFill;

        public SimAgent sim;

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

            sleepBarFill.color = sim.sleep < sim.threshold ? Color.red : Color.green;
            hungerBarFill.color = sim.hunger < sim.threshold ? Color.red : Color.green;
            waterBarFill.color = sim.water < sim.threshold ? Color.red : Color.green;
        }
    }
}
