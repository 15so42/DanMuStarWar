using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : BattleUnit
{
    public float eatDistance = 4;
    public override void Attack()
    {
        if (Vector3.Distance(transform.position, chaseTarget.GetVictimEntity().transform.position) < eatDistance)
        {
            chaseTarget.OnAttacked(new AttackInfo(this,AttackType.Physics,10));
        }
    }
}
