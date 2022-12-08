using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWeapon : HandWeapon
{
    public override void FireAnim()
    {
        var distance =
            Vector3.SqrMagnitude(owner.chaseTarget.GetVictimEntity().transform.position - transform.position);

        if (distance < 10)
        {
            animator.SetTrigger("Attack");
            Invoke(nameof(MeleeAttack),0.5f);
        }
        else
        {
            animator.SetTrigger("DragonBreath");
        }
        
    }

    public void MeleeAttack()
    {
        var enemys = AttackManager.Instance.GetEnemyInRadius(owner, transform.position, 10,9);
        foreach (var victim in enemys)
        {
            DamageOther(victim,new AttackInfo(owner,AttackType.Real,Mathf.CeilToInt(victim.GetVictimEntity().props.hp*0.15f)));

            var victimEntity = victim.GetVictimEntity();
            var navMove = victimEntity.GetComponent<NavMeshMoveManager>();
            if (navMove)
            {
                navMove.PushBackByPos( victimEntity.transform.position,transform.position,12,9,1);
            }

        }
    }
}
