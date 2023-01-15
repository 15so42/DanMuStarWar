using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : HandWeapon
{
    public GameObject axeBulletPfb;

    private float lastFlyAxeTime=0;
    
    
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (owner.IsTargetAlive() && Time.time >=lastFlyAxeTime+15 )
        {
            GameObject axeGo = GameObject.Instantiate(axeBulletPfb);
            var bullet = axeGo.GetComponent<ArrowBullet>();
            axeGo.transform.position = transform.position;
            Vector3 dir = owner.chaseTarget.GetVictimPosition() - transform.position;
            bullet.Init(owner,dir,this);
            lastFlyAxeTime = Time.time;
            animator.SetTrigger("Attack");
        }
    }
}
