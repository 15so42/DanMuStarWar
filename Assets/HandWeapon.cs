using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HandWeapon : Weapon
{
    private Transform root;
    [Header("手动配置Animator")] public Animator animator;
    private void Start()
    {
        root = transform.root;
        animator = root.GetComponent<BattleUnit>().animator;
        if(animator==null)
            Debug.Log(name+"需要手动配置");
    }

    public override bool FireCheck()
    {
        var distance =
            Vector3.Distance(root.transform.position, owner.chaseTarget.GetVictimEntity().transform.position);
        if (distance < attackDistance)
        {
            return true;
        }

        return false;
    }

    public override void Fire()
    {
        animator.SetTrigger("Attack");
        owner.chaseTarget.GetVictimEntity().OnAttacked(new AttackInfo(this.owner,AttackType.Physics,1));
    }
}
