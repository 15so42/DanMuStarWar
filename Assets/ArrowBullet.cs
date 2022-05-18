using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using UnityEngine;

public class ArrowBullet : MonoBehaviour
{
    public IAttackAble owner;
    //private int damageValue;
    public int speed = 30;
    public int strength=1;

    private Rigidbody rigidbody;

    List<IVictimAble> attacked=new List<IVictimAble>();//储存伤害过单位列表，不能多次伤害同一单位

    protected RecycleAbleObject recycleAbleObject;


    private void Awake()
    {
        recycleAbleObject = GetComponent<RecycleAbleObject>();
    }

    public void Init(IAttackAble owner,Vector3 dir,int strength)
    {
        this.owner = owner;
        transform.forward = dir;
        rigidbody= GetComponent<Rigidbody>();
        
        
        rigidbody.AddForce(dir.normalized*speed);
        this.strength = strength;
        

    }
    //strength决定伤害和击退距离
    private void OnCollisionEnter(Collision other)
    {
        if(gameObject==null || rigidbody==null)
            return;
        if(rigidbody.velocity.magnitude<1)
            return;
        
        
        var victim = other.gameObject.GetComponent<IVictimAble>();
        if(victim==null)
            return;
        
        victim =victim.GetVictimEntity();
        if(victim==this.owner || attacked.Contains(victim))
            return;
        
        //取消友伤
        if(victim.GetVictimOwner()==owner.GetAttackerOwner())
            return;
        victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,3*strength));
        var navMeshMoveManager = victim.GetGameObject().GetComponent<NavMeshMoveManager>();
        if(navMeshMoveManager)
            navMeshMoveManager.PushBack(victim.GetGameObject().transform.position-transform.position+Vector3.up*strength*4,strength);
        attacked.Add(victim);
        recycleAbleObject.Recycle();
    }

    private void OnDisable()
    {
        rigidbody.velocity=Vector3.zero;
        attacked.Clear();
    }
}
