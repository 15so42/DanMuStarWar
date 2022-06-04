using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class BoomerangBullet : ArrowBullet
{
    private float timer = 0;

    private Vector2 dir;


    private TrailRenderer[] trailRenderers;
    private void Awake()
    {
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
        
    }


    public override void Init(IAttackAble owner, Vector3 dir, HandWeapon handWeapon)
    {
        base.Init(owner, dir, handWeapon);
        this.dir = dir;
        //rigidbody.isKinematic = true;
        timer = 0;
        for (int i = 0; i < trailRenderers.Length; i++)
        {
            trailRenderers[i].Clear();
        }
       
    }

    private Vector3 refPosition;
    void Update()
    {
        var lastTimer = timer;
        timer += Time.deltaTime;
        if(owner==null || owner.GetAttackEntity()==null)
            return;
        if (timer > 3)
        {
            if (lastTimer <= 3 && timer > 3)
            {
                OnStartBack();
            }
            rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, Vector3.zero, 1*Time.deltaTime);
            // transform.position = Vector3.Slerp(transform.position, owner.GetAttackEntity().transform.position+Vector3.up*5,
            //      1*Time.deltaTime);
            transform.position+=(owner.GetAttackEntity().transform.position+Vector3.up*5-transform.position).normalized * (speed*0.07f * Time.deltaTime);

            Vector3 distanceDir = transform.position - owner.GetAttackEntity().transform.position;
            distanceDir.y = 0;
            if (distanceDir.magnitude < 3f||timer>10)
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

    

    void OnStartBack()
    {
        attacked.Clear();
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
