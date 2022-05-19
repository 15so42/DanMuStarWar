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

    protected Rigidbody rigidbody;

    List<IVictimAble> attacked=new List<IVictimAble>();//储存伤害过单位列表，不能多次伤害同一单位

    protected RecycleAbleObject recycleAbleObject;


    //弓箭伤害=3+力量等级*2,力量等级也带有更强的击退效果
    private void Awake()
    {
        recycleAbleObject = GetComponent<RecycleAbleObject>();
    }

    public virtual void Init(IAttackAble owner,Vector3 dir)
    {
        this.owner = owner;
        transform.forward = dir;
        rigidbody= GetComponent<Rigidbody>();

        rigidbody.AddForce(dir.normalized*speed);
    }
    

    public void SetStrength(int strength)
    {
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
        victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,3+strength*2));
        var navMeshMoveManager = victim.GetGameObject().GetComponent<NavMeshMoveManager>();
        if(navMeshMoveManager)
            navMeshMoveManager.PushBack(victim.GetGameObject().transform.position-transform.position+Vector3.up*strength*4,strength);
        attacked.Add(victim);
        recycleAbleObject.Recycle();
    }

    public virtual void OnDisable()
    {
        rigidbody.velocity=Vector3.zero;
        attacked.Clear();
        strength = 0;
    }
}
