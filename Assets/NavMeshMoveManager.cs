using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NavMeshMoveManager : MoveManager
{

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void SetFinalTarget(Vector3 finalTarget, bool syncTmpTarget)
    {
        base.SetFinalTarget(finalTarget, syncTmpTarget);
        CustomEvent.Trigger(gameObject, "OnDestinationSet");
    }

    public override void SetTmpTarget(Vector3 tmpTarget)
    {
        base.SetTmpTarget(tmpTarget);
        navMeshAgent.SetDestination(tmpTarget);
    }

    protected override void Update()
    {
        //DoNothing
    }
}
