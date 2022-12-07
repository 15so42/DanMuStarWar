using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWeapon : HandWeapon
{
    private static readonly int Shield = Animator.StringToHash("Shield");


    public override AttackInfo OnBeforeAttacked(AttackInfo attackInfo)
    {
        var attackInfo1=base.OnBeforeAttacked(attackInfo);
        
        var attacker = attackInfo.attacker?.GetAttackEntity();
        var ignoreDamageType = new List<AttackType>() {AttackType.Poison,AttackType.Fire,AttackType.Reflect,AttackType.Heal};
        if (attacker != null &&  !ignoreDamageType.Contains(attackInfo.attackType))
        {
            var toReduceEndurance = attackInfo1.value;
            
            //在前方，伤害由盾牌承受
            if (Vector3.Dot(owner.transform.forward, attacker.transform.position-owner.transform.position) > 0)
            {
               
                
            }
            else
            {
                toReduceEndurance = (int)(toReduceEndurance * 1.5f);
            }
            
           
            if (endurance >= attackInfo1.value)
            {
                var hardLevel = GetWeaponLevelByNbt("坚硬");

                
                if (hardLevel > 0)
                {
                    toReduceEndurance = Mathf.CeilToInt(toReduceEndurance * (1-(float)hardLevel / (hardLevel + 10)));
                }
                AddEndurance(-1*toReduceEndurance);
                FlyText.Instance.ShowDamageText(owner.transform.position-Vector3.up*3,"耐久-"+toReduceEndurance);
                return new AttackInfo(attackInfo.attacker,attackInfo.attackType,0);
            }
        }

        return attackInfo;

    }

    public override void FireAnim()
    {
        animator.SetTrigger("ShieldAttack");
        
        Invoke(nameof(DamageChaseTarget),0.3f);
    }


    protected override void Update()
    {
        base.Update();
        animator.SetBool(Shield,false);
        if (owner.IsTargetAlive())
        {
            float sqrDistance = Vector3.SqrMagnitude(owner.transform.position-owner.chaseTarget.GetVictimEntity().transform.position);
            if (sqrDistance > attackDistance * attackDistance)
            {
                animator.SetBool(Shield,true);
            }
        }
    }
}
