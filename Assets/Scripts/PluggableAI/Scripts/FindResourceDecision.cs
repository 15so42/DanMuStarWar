using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Decisions/FindResPlanet")]
public class FindResourceDecision : Decision
{
    

    public override bool Decide(StateController controller)
    {
        var seekerStateController = controller;
        if (seekerStateController.chaseTarget)
        {
            return true;
        }
        var list=GameManager.Instance.planetManager.allPlanets;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].GetOwner() ==null && list[i].planetResMgr.HasAnyRes())
            {
                seekerStateController.chaseTarget = list[i].transform;
                return true;
            }
        }

        return false;
    }
}
