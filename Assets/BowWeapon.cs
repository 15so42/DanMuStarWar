using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;
using UnityEngine.AI;

public class BowWeapon : HandWeapon
{
    public Transform shootPoint;
    public int bowStrength;
    
    
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
        arrowComp.Init(owner,owner.chaseTarget.GetVictimEntity().transform.position-transform.position ,1);
    }

}
