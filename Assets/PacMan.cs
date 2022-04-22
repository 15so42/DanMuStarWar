using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
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
        if(die)
            return;
        //
        //     chaseTarget.OnAttacked(new AttackInfo(this,AttackType.Physics,10));
        // }

        if (Vector3.Distance(transform.position, chaseTarget.GetVictimEntity().transform.position) < eatDistance)
        {
            Explosion();
            
            Die();
        }
    }

    private void OnDisable()
    {
        dieTimer?.Cancel();
    }

    public override void Die()
    {
        Explosion();
        base.Die();
    }

    void Explosion()
    {
        var attackInfo = new AttackInfo(this, AttackType.Physics, 20);
        var position = transform.position;
        AttackManager.Instance.Explosion(attackInfo, position, 30);
    }
}
