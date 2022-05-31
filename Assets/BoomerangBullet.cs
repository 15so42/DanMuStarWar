using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class BoomerangBullet : ArrowBullet
{
    private float timer = 0;

    private Vector2 dir;
    private void OnEnable()
    {
        //throw new NotImplementedException();
       
    }

    public override void Init(IAttackAble owner, Vector3 dir, HandWeapon handWeapon)
    {
        base.Init(owner, dir, handWeapon);
        this.dir = dir;
        //rigidbody.isKinematic = true;
        timer = 0;
    }

    private Vector3 refPosition;
    void Update()
    {
       
        timer += Time.deltaTime;
        if (timer > 3)
        {
            rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, Vector3.zero, 1*Time.deltaTime);
            transform.position = Vector3.SmoothDamp(transform.position, owner.GetAttackEntity().transform.position+Vector3.up*5,
                ref refPosition, 1);

            Vector3 distanceDir = transform.position - owner.GetAttackEntity().transform.position;
            distanceDir.y = 0;
            if (distanceDir.magnitude < 3f)
            {
                timer = 0;
                recycleAbleObject.Recycle();
                (handWeapon as BoomerangeWeapon).OnBoomerangeBack();//飞回后才能进行下一次的攻击
            }
        }
        else
        {
            //transform.Translate(dir * (speed * Time.deltaTime),Space.World);
        }
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var victim = base.ValidCheck(other);
        if(victim==null)
            return;
        DamageVictim(victim);
        DamageFx(victim);
    }
}
