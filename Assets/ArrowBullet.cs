using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using UnityEngine;





public class ArrowBullet : MonoBehaviour
{
    public IAttackAble owner;

    public HandWeapon handWeapon;
    //private int damageValue;
    public int speed = 30;
    public int strength=1;

    protected Rigidbody rigidbody;

    protected List<IVictimAble> attacked=new List<IVictimAble>();//储存伤害过单位列表，不能多次伤害同一单位

    public bool recycleOnCollision = true;

    protected RecycleAbleObject recycleAbleObject;

    public bool damageOnce = true;


    //弓箭伤害=3+力量等级*2,力量等级也带有更强的击退效果
    private void Awake()
    {
        recycleAbleObject = GetComponent<RecycleAbleObject>();
    }

    public virtual void Init(IAttackAble owner,Vector3 dir,HandWeapon handWeapon)
    {
        this.owner = owner;
        transform.forward = dir;
        rigidbody= GetComponent<Rigidbody>();
        this.handWeapon = handWeapon;

        rigidbody.AddForce(dir.normalized*speed);
    }
    

    public void SetStrength(int strength)
    {
        this.strength = strength;
    }


    protected IVictimAble ValidCheck(Collider other)
    {
        if(gameObject==null || rigidbody==null)
            return null;
        // if(rigidbody.velocity.magnitude<1)
        //     return null;
        
        var victim = other.gameObject.GetComponent<IVictimAble>();
        if(victim==null)
            return null;
        
        victim =victim.GetVictimEntity();
        if(victim==this.owner || attacked.Contains(victim))
            return null;
        
        //取消友伤
        if(victim.GetVictimOwner()==owner.GetAttackerOwner())
            return null;
        return victim;
    }
    

    //strength决定伤害和击退距离
    public virtual void OnCollisionEnter(Collision other)
    {
        var victim = ValidCheck(other.collider);
        if(victim==null)
            return;

       
        // var hpAndShield = victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,3+strength*2));
        // handWeapon.OnDamageOther(victim,hpAndShield);
        // attacked.Add(victim);
        DamageVictim(victim);
        
        // var navMeshMoveManager = victim.GetGameObject().GetComponent<NavMeshMoveManager>();
        // if(navMeshMoveManager)
        //     navMeshMoveManager.PushBackByPos(victim.GetGameObject().transform.position,owner.GetAttackerOwner().transform.position,3,2,1+strength*0.2f);
        //
        //
        // if (recycleAbleObject)
        // {
        //     recycleAbleObject.Recycle();
        // }
       
        DamageFx(victim);
            
        
    }

    public virtual int CalDamage()
    {
        return 3 + strength * 2;
    }
    protected void DamageVictim(IVictimAble victim)
    {
        var hpAndShield = victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,CalDamage()));
        handWeapon.OnDamageOther(victim,hpAndShield);
        if(damageOnce)
            attacked.Add(victim);
    }

    protected void DamageFx(IVictimAble victim)
    {
        var navMeshMoveManager = victim.GetGameObject().GetComponent<NavMeshMoveManager>();
        if(navMeshMoveManager)
            navMeshMoveManager.PushBackByPos(victim.GetGameObject().transform.position,owner.GetAttackerOwner().transform.position,3,2,1+strength*0.2f);
        

        if (recycleAbleObject && recycleOnCollision)
        {
            recycleAbleObject.Recycle();
        }
        
    }

    public virtual void OnDisable()
    {
        rigidbody.velocity=Vector3.zero;
        attacked.Clear();
        strength = 0;
    }
}
