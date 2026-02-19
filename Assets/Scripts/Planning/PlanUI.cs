using NUnit.Framework;
using Planning;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanUI : MonoBehaviour
{

    public TextMeshProUGUI goalTxt;
    public TextMeshProUGUI planTxt;
    public TextMeshProUGUI statsTxt;


    public void SetGoalTxt(string txt)
    {
        goalTxt.text = txt;
    }

    public void SetPlanTxt(List<PlanAction> plan)
    {
        planTxt.text = "==== <b>PLAN</b> ====\n";

        if (plan == null || plan.Count == 0)
        {
            planTxt.text += "No plan :(";
        }

        for (int i = 0; i < plan.Count; i++)
        {
            planTxt.text += $"{i + 1}. {plan[i].ToString()} \n";
        }
        planTxt.text += $"\n---- {plan.Count} steps ----";
    }

    public void SetStatsTxt(string txt)
    {
        statsTxt.text = txt;
    }

    public void ShowPlan(PlanResult plan)
    {
        SetPlanTxt(plan.plan);
        SetStatsTxt($"Time: {plan.TimeMs}ms" +
            $"\nSteps: {plan.Steps}" +
            $"\nDepth: {plan.Depth}");
    }
}
