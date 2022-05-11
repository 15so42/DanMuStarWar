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

    [Header("弓箭向上随机提前量")]
    public int minUpRate;
    public int maxUpRate;
    
    public override void Fire()
    {
        animator.SetTrigger("BowAttack");
        Invoke(nameof(ShootArrow),0.6f);
        
    }

    public void ShootArrow()
    {
        GameObject arrowBullet =
            ResFactory.Instance.CreateBullet(GameConst.BULLET_ARROW, shootPoint.transform.position);
        var arrowComp = arrowBullet.GetComponent<ArrowBullet>();
        var dir = Vector3.forward;

        var targetPos = owner.chaseTarget.GetVictimEntity().transform.position +
                        Vector3.up * UnityEngine.Random.Range(0, 10);

        dir = targetPos - transform.position;
        
        Debug.DrawRay(transform.position,dir);
        arrowComp.Init(owner, dir,1);
    }
    
    

}
