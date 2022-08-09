using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;
using UnityEngine.AI;

public class BowWeapon : HandWeapon
{
    public Transform shootPoint;
    [Header("弓箭附魔力量等级")]
    

    public string bulletName="ArrowBullet";
    public string animTriggerName = "BowAttack";
    public float invokeDelay = 0.6f;

    [Header("偏差高度")]
    public int minHeight = 3;
    public int maxHeight = 7;

    public override float GetAttackSpeed()
    {
        var attackSpeedLevel = GetWeaponLevelByNbt("速射");
        return attackSpeed + attackSpeedLevel * 0.07f;
        
    }
 
    
    public override void FireAnim()
    {
        animator.SetTrigger(animTriggerName);

        Invoke(nameof(ShootArrow),invokeDelay);
    }

    // public override void Fire()
    // {
    //     animator.SetTrigger(animTriggerName);
    //
    //     Invoke(nameof(ShootArrow),invokeDelay);
    //     endurance--;
    //     OnEnduranceChange(endurance,maxEndurance);
    //     
    // }

    public virtual T InitBullet<T>(string bulletName) where T:Component
    {
        GameObject arrowBullet =
            ResFactory.Instance.CreateBullet(bulletName, shootPoint.transform.position);
        T arrowComp = arrowBullet.GetComponent<T>();
        return arrowComp;
    }

    public  void ShootArrow()
    {
        if(owner.IsTargetAlive()==false)
            return;
        
        var arrowComp=InitBullet<ArrowBullet>(bulletName);
        var dir = Vector3.forward;

        var targetPos=transform.position+transform.forward+Vector3.up * UnityEngine.Random.Range(0, 7);
        if (owner.chaseTarget != null)
        {
            targetPos = owner.chaseTarget.GetVictimEntity().transform.position +
                        Vector3.up * UnityEngine.Random.Range(minHeight, maxHeight);
        }
        
       
        // targetPos = owner.chaseTarget.GetVictimEntity().transform.position +
        //                 Vector3.up * UnityEngine.Random.Range(0, 7);

        dir = targetPos - transform.position;
        
        Debug.DrawRay(transform.position,dir);

      
       
        InitArrow(arrowComp,dir);
        
        //arrowComp.Init(owner, dir,strength);
    }
    
    

    public virtual void InitArrow(ArrowBullet arrowComp,Vector3 dir)
    {
        var bowStrength = GetWeaponLevelByNbt("力量");
        arrowComp.Init(owner, dir,this);
        arrowComp.SetStrength(bowStrength);
        arrowComp.SetQuickShoot(GetWeaponLevelByNbt("速射"));
    }

  
    
    

}
