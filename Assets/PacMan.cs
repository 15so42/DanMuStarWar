using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : BattleUnit
{
    public float eatDistance = 4;

    private UnityTimer.Timer dieTimer;
    private void OnEnable()
    {
        dieTimer = UnityTimer.Timer.Register(180, Die);
    }

    public override void Attack()
    {
        if (Vector3.Distance(transform.position, chaseTarget.GetVictimEntity().transform.position) < eatDistance)
        {
            chaseTarget.OnAttacked(new AttackInfo(this,AttackType.Physics,10));
        }
    }

    private void OnDisable()
    {
        dieTimer?.Cancel();
    }
}
