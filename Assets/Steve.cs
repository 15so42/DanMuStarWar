using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.Rendering;

public class Steve : WarPlane
{
    public float attackDistance;
    
    public int ownerPosIndex;

    public Vector3 targetMcPos = Vector3.zero;

    private FightingManager fightingManager;

    public SkinnedMeshRenderer[] meshRenderers;
    
    [Header("随机武器列表检测")]
    public List<GameObject> mcWeapons=new List<GameObject>();
    
    protected override void Start()
    {
        base.Start();
       
        fightingManager = GameManager.Instance.fightingManager;
        moveManager = GetComponent<MoveManager>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        RandomWeapon();
        hpUI.SetNameText(planetCommander.player.userName);
    }

    public void RandomWeapon()
    {
       
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        var liveWeapon=weapons[UnityEngine.Random.Range(0,weapons.Count)];
        
        liveWeapon.gameObject.SetActive(true);
        liveWeapon.Init(this);
    }
    
    public Vector3 GetPos(int index)
    {
        return fightingManager.mcPosManager.GetPosByIndex(index);
    }

    public override void GoMCPos(Vector3 pos)
    {
        base.GoMCPos(pos);

        targetMcPos = pos;
        CustomEvent.Trigger(gameObject, "OnDestinationSet");
    }

   
    public bool NearTargetPos()
    {
        float distance = Vector3.Distance(transform.position, moveManager.tmpTarget);
        return distance < 1f;
    }

    ///
    /// 朝向敌人
    ///
    public void RotateToChaseTarget()
    {
        var targetDir = chaseTarget.GetVictimEntity().transform.position - transform.position;
        targetDir.y = 0;
        transform.forward=Vector3.Lerp(transform.forward,targetDir,2 * Time.deltaTime);
    }
    
    
    /// <summary>
    /// 装备武器或者更换武器时调用,仅用于设置状态机相关判定数值，
    /// </summary>
    public void SetAttackDistance(float value)
    {
        this.attackDistance = value;
    }

    public bool IsInAttackRange()
    {
        float distance = Vector3.Distance(chaseTarget.GetVictimEntity().transform.position, transform.position);
        if (distance < attackDistance)
            return true;
        return false;
    }

    public override IVictimAble EnemyCheck(Collider collider)
    {
        var gameEntity = collider.GetComponent<GameEntity>();
        if (!gameEntity)//不是单位
            return null;

        var gameEntityOwner = gameEntity.GetVictimOwner();
        if (gameEntity==null || gameEntityOwner == GetAttackerOwner()) //同星球
            return null;
        if (gameEntity.die)//已经死亡
            return null;

        var targetPlanet = gameEntityOwner as Planet;
        if(targetPlanet==null )//如果只对敌对星球寻敌，而敌对星球不存在，或找到的单位不属于，不算作敌人
            return null;
            
        if (ownerPlanet.enemyPlanets.Contains(targetPlanet))
        {
            return gameEntity;
        }

        return null;


    }
    
    private void Update()
    {
        //throw new NotImplementedException();
    }

    //进入防御点位后，武器的攻击距离增加
    public void EnterDefendState(int value)
    {
        float minDistance = 10000;
        foreach (var w in weapons)
        {
            if(w.gameObject.activeSelf==false)
                continue;
            if (w.addAtkDistanceByDP)
            {
                w.attackDistance += value;
            }
            if (w.attackDistance < minDistance)
                minDistance = w.attackDistance;
            
        }
        SetAttackDistance(minDistance);
    }

    public void ExitDefendState(int value)
    {
        float minDistance = 10000;
        foreach (var w in weapons)
        {
            if(w.gameObject.activeSelf==false)
                continue;
            if (w.addAtkDistanceByDP)
            {
                w.attackDistance -= value;
            }
            if (w.attackDistance < minDistance)
                minDistance = w.attackDistance;
                
            SetAttackDistance(minDistance);
        }
    }
    

    /// <summary>
    /// 受击特效
    /// </summary>
    /// <param name="attackInfo"></param>
    public override void OnAttacked(AttackInfo attackInfo)
    {
        base.OnAttacked(attackInfo);
        
        if(attackInfo.attackType==AttackType.Heal)
            return;
        
        StopAllCoroutines();
        if (gameObject.activeSelf)
        {
            StartCoroutine(VictimFx());
        }
        

    }

   

    IEnumerator VictimFx()
    {
        
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.EnableKeyword("_EMISSION");
            meshRenderers[i].material.SetColor("_EmissionColor",new Color(1,0,0));
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.SetColor("_EmissionColor",new Color(0,0,0));
        }
    }

    public override void Die()
    {
        EventCenter.Broadcast(EnumEventType.OnSteveDied,this);
        base.Die();
    }
}
