using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Move")]
public class MoveAction : AIAction
{
    public override void Act(StateController controller)
    {
        controller.moveManager.SetFinalTarget(controller.chaseTarget.position);
    }
}
