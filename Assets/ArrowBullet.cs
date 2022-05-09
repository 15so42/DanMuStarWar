using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBullet : MonoBehaviour
{
    public IAttackAble owner;
    //private int damageValue;
    public int speed = 30;
    public int strength=1;
    
    public void Init(IAttackAble owner,Vector3 dir,int strength)
    {
        this.owner = owner;
        transform.forward = dir;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(dir.normalized*speed);
        this.strength = strength;
    }
    //strength决定伤害和击退距离
    private void OnCollisionEnter(Collision other)
    {
        var victim = other.gameObject.GetComponent<IVictimAble>();
        if(victim==null)
            return;
        
        victim =victim.GetVictimEntity();
        if(victim==this.owner)
            return;
        victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,1*strength));
        var navMeshMoveManager = victim.GetGameObject().GetComponent<NavMeshMoveManager>();
        if(navMeshMoveManager)
            navMeshMoveManager.PushBack(victim.GetGameObject().transform.position-transform.position+Vector3.up*strength*4,strength);

    }
}
