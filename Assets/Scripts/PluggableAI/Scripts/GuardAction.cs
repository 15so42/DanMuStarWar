using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "PluggableAI/Actions/Guard")]
public class GuardAction : AIAction
{
    public override void Act(StateController controller)
    {

        controller.moveManager.Guard(controller.planet);

    }
}
