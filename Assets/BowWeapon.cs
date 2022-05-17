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
    public int bowStrength;

    public string bulletName="ArrowBullet";
    public string animTriggerName = "BowAttack";
    public float invokeDelay = 0.6f;
    
    public override void Fire()
    {
        animator.SetTrigger(animTriggerName);
        Invoke(nameof(ShootArrow),invokeDelay);
        endurance--;
        OnEnduranceChange(endurance,maxEndurance);
        
    }

    public virtual T InitBullet<T>(string bulletName) where T:Component
    {
        GameObject arrowBullet =
            ResFactory.Instance.CreateBullet(bulletName, shootPoint.transform.position);
        T arrowComp = arrowBullet.GetComponent<T>();
        return arrowComp;
    }

    public  void ShootArrow()
    {
        
        var arrowComp=InitBullet<ArrowBullet>(bulletName);
        var dir = Vector3.forward;

        var targetPos=transform.position+transform.forward+Vector3.up * UnityEngine.Random.Range(0, 7);
        if (owner.chaseTarget != null)
        {
            targetPos = owner.chaseTarget.GetVictimEntity().transform.position +
                        Vector3.up * UnityEngine.Random.Range(0, 7);
        }
        // targetPos = owner.chaseTarget.GetVictimEntity().transform.position +
        //                 Vector3.up * UnityEngine.Random.Range(0, 7);

        dir = targetPos - transform.position;
        
        Debug.DrawRay(transform.position,dir);
        arrowComp.Init(owner, dir,1);
    }

  
    
    

}
