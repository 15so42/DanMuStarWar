using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using UnityEngine;

public class TntBullet : ArrowBullet
{
    private Material material;

    private bool sticky = false;
    private int highExplosiveLevel = 0;

    public IVictimAble followTarget;
    private bool lastTargetStatus=false;
   
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        material.EnableKeyword("_EMISSION");
        
    }

    public override void Init(IAttackAble owner, Vector3 dir, HandWeapon handWeapon)
    {
        base.Init(owner, dir, handWeapon);
        rigidbody.velocity=Vector3.zero;
    }

    public void SetSticky(bool status)
    {
        this.sticky = status;
    }

    public void SetHighExplosive(int level)
    {
        this.highExplosiveLevel = level;
    }

    public void SetFollowTarget(IVictimAble target)
    {
        this.followTarget = target;
    }
    
    

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(TNTExplosion());
    }


    private void OnCollisionEnter(Collision other)
    {
        // if(other.collider.gameObject==owner.GetAttackEntity().gameObject)
        //     return;
        //
        // if (sticky)
        // {
        //     transform.SetParent(other.transform);
        //     rigidbody.isKinematic = true;
        // }
    }

    private void Update()
    {
        var dieState = followTarget==null || followTarget.GetVictimEntity().die;//是否已经嗝屁
        if (dieState != lastTargetStatus)
        {
            if (dieState)//突然嗝屁
            {
                rigidbody.velocity=Vector3.zero;
            }
            // else//初次设置敌人
            // {
            //     transform.DOMove(followTarget.GetVictimEntity().transform.position + Vector3.up * 12f,0.5f);
            // }
                
        }

        if (!dieState)
        {
            transform.position = followTarget.GetVictimEntity().transform.position + Vector3.up * 12f;
        }
        
    }

    IEnumerator TNTExplosion()
    {
        yield return new WaitForSeconds(2);
        
        material.SetColor("_EmissionColor",new Color(1,0,0));
        yield return new WaitForSeconds(0.5f);
        material.SetColor("_EmissionColor",new Color(0,0,0));
        yield return new WaitForSeconds(1);
        
        material.SetColor("_EmissionColor",new Color(1,0,0));
        yield return new WaitForSeconds(0.5f);
        material.SetColor("_EmissionColor",new Color(0,0,0));
        yield return new WaitForSeconds(1);

        ExplosionDamage();
        //ExplosionFx();

    }

    void ExplosionDamage()
    {
            
            var attackInfo = new AttackInfo(owner, AttackType.Physics, 5+highExplosiveLevel*1);
            var position = transform.position;
            AttackManager.Instance.Explosion(attackInfo,handWeapon, position, 15,"MCExplosionFx");
            //Debug.Log(gameObject.name+"Explosion一次");
            recycleAbleObject.Recycle();
        
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SetSticky(false);
        rigidbody.isKinematic = false;
        highExplosiveLevel = 0;
        followTarget = null;
        lastTargetStatus = false;
    }
}
