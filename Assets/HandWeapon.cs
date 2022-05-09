using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class HandWeapon : Weapon
{
    private Transform root;
    [HideInInspector] public Animator animator;

    //public int stoppingDistance=5;

    

    [Header("击飞高度和力度")]
    public int pushBackHeight=4;
    public int pushBackStrength=1;
    public virtual void Start()
    {
        root = transform.root;
        animator = root.GetComponent<BattleUnit>().animator;
        if(animator==null)
            Debug.Log(name+"需要手动配置");
        (owner as Steve).SetAttackDistance(attackDistance);
        //root.GetComponent<NavMeshAgent>().stoppingDistance = stoppingDistance;
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
        var victim = owner.chaseTarget.GetVictimEntity();
        victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,1));
        var navMeshMoveManager = victim.GetComponent<NavMeshMoveManager>();
        if(navMeshMoveManager)
            navMeshMoveManager.PushBack(victim.transform.position-transform.position+Vector3.up*pushBackHeight,pushBackStrength);
    }

    
}
