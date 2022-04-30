﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class Weapon : MonoBehaviour
{
    public float attackDistance;
    public BattleUnit owner;
    
    
    //武器攻速计时
    public float attackSpeed = 1;
    private float timer = 0;
    private bool ready = true;
    //散射角度，当检测方式为ray时，射击向量在玩家到敌人的方向进行对应度数的随机取值以模拟误差射击效果，如若射中玩家或者障碍物，发射子弹到对应位置并附带拖尾
    [Header("散射角度")]
    public float scatteringAngel=5;

    public float rayDistance = 20;

    [Header("伤害配置")] public AttackType attackType;
    
    public int attackValue = 1;
   
    public void Init(BattleUnit owner)
    {
        this.owner = owner;

        
        timer = 1 / attackSpeed;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            ready = true;
            
            
        }
    }

    public void Attack()
    {
        if (ready)
        {
            if (FireCheck())
            {
                Fire();
                ready = false;
                timer=1 / attackSpeed;
            }
            
        }
    }
    public virtual void Fire()
    {
        if(owner.chaseTarget.GetVictimEntity().die)
            return;
        var dir = owner.chaseTarget.GetVictimEntity().transform.position - transform.position;
        var distance = Vector3.Distance(owner.chaseTarget.GetVictimEntity().transform.position, transform.position);
        if ( distance< attackDistance)
        {
          
            //散射算法
            Vector3 newVec = Quaternion.Euler(Random.Range(-scatteringAngel,scatteringAngel),Random.Range(-scatteringAngel,scatteringAngel),0)*dir;
            //PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,newVec);
                
            RaycastHit hitInfo=new RaycastHit();
            
            if (Physics.Raycast(transform.position, newVec,out hitInfo, rayDistance))
            {
                var prop = hitInfo.collider.GetComponent<BattleUnitProps>();
                var attackerOwner = owner.GetAttackerOwner();
                if(prop==null || prop.gameEntity==null)
                    return;
                var victimOwner = prop.gameEntity.GetVictimOwner();
                if(victimOwner==null || attackerOwner==victimOwner)
                    return;//避免友军伤害
                if (prop && prop.gameEntity!=owner && prop.gameEntity)
                {
                    prop.gameEntity.OnAttacked(new AttackInfo(owner,attackType,attackValue));
                }
               
                FireFx(transform.position,hitInfo);
            }
            else
            {
                
                FireFx(transform.position,transform.position+dir*rayDistance);
            }
        }
    }

    public virtual bool FireCheck()
    {
        return true;
    }

    public virtual void FireFx(Vector3 startPos,RaycastHit hitInfo)
    {
        FireFx(startPos,hitInfo.point);
    }

    public virtual void FireFx(Vector3 startPos, Vector3 endPos)
    {
       
    }
}
