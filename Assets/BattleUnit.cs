using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;
using Random = UnityEngine.Random;

[IncludeInSettings(true)]
public class BattleUnit : GameEntity,IAttackAble,IVictimAble
{
   
    public Planet ownerPlanet;
    //指挥官
    public PlanetCommander planetCommander;

    protected MoveManager moveManager;

    public GameManager gameManager;
    
    [HideInInspector] public BattleUnitManager battleUnitManager;
    [HideInInspector] public PlanetManager planetManager;

    public float findEnemyDistance = 7;
    public IVictimAble chaseTarget = null;

    [Header("可进攻单位")] 
    public bool canAttack = true;

    public bool inWar = false;
    
    [Header("可驻守")]
    public bool canDefendOtherPlanet=true;

    public Planet defendingPlanet;

    public bool isDefending;
    
    //动画
    [HideInInspector]public Animator animator;
    
    protected void Awake()
    {
        base.Awake();
        moveManager = GetComponent<MoveManager>();
       
        
        
        gameManager=GameManager.Instance;
        planetManager = gameManager.planetManager;
        battleUnitManager = gameManager.battleUnitManager;
        animator = GetComponentInChildren<Animator>();
        
    }

    public bool IsTargetAlive()
    {
        var target = chaseTarget?.GetVictimEntity();
        return chaseTarget != null && target.IsAlive() ;
    }

    public bool IsInFindRange()
    {
        var target = chaseTarget?.GetVictimEntity();
        return target != null  && !target.die && Vector3.Distance(target.transform.position,transform.position)<findEnemyDistance;
    }

    
    /// <summary>
    /// MC模式表示前往某地
    /// </summary>
    public virtual void GoMCPos(Vector3 pos)
    {
        
    }

    public virtual IVictimAble EnemyCheck(Collider collider)
    {
        return null;
    }

    protected void Start()
    {
       base.Start();
       if (showHpUI)
       {
           if (ownerPlanet)
           {
               hpUI.SetColor(ownerPlanet.planetColor);
           }
           else
           {
               hpUI.SetColor(Color.white);
           }
           if(planetCommander!=null)
             hpUI.SetColor(planetCommander.color);
       }
       
       
       
       
       EventCenter.Broadcast(EnumEventType.OnBattleUnitCreated,this);
        //SkillManager.Instance.AddSkill("Skill_腐蚀_LV1",this);
    }

    public bool IsEntityPlanet(GameEntity entity)
    {
        if (entity != null && entity.GetVictimEntity().GetComponent<Planet>() && !entity.die)
        {
            return true;
        }

        return false;
    }

    //守卫状态寻敌事件
    public void OnBattleUnitEnterPlanet(BattleUnit entrant, Planet planet)
    {
        if (isDefending && defendingPlanet == planet || ownerPlanet==planet)
        {
            if(entrant.die || GetAttackerOwner()==entrant.GetVictimOwner())
                return;
            SetChaseTarget(entrant);
        }
    }
    

    //不需要宣战状态，宣战后直接向目标星球前进，途中通过此函数寻找最近敌人，没找到的话直接进攻星球，否则性能消耗太高
    //固定距离寻敌，
    public virtual GameEntity OverLapEnemy(bool onlyEnemyPlanet=true)
    {
        var colliders = Physics.OverlapSphere(transform.position, findEnemyDistance);
        foreach (var collider in colliders)
        {
            var gameEntity = collider.GetComponent<GameEntity>();
            if (!gameEntity)//不是单位
                continue;

            var gameEntityOwner = gameEntity.GetVictimOwner();
            if (gameEntity==null || gameEntityOwner == GetAttackerOwner()) //同星球
                continue;
            if (gameEntity.die)//已经死亡
                continue;

            var targetPlanet = gameEntityOwner as Planet;
            if(targetPlanet==null )//如果只对敌对星球寻敌，而敌对星球不存在，或找到的单位不属于，不算作敌人
                continue;
            
            if (onlyEnemyPlanet && ownerPlanet.enemyPlanets.Contains(targetPlanet))
            {
                return gameEntity;
            }

            if (!onlyEnemyPlanet)
            {
                return gameEntity;
            }
            

            

        }

        return null;
    }
    
    /// <summary>
    /// 自动判断是否war状态
    /// </summary>
    /// <param name="onlyEnemyPlanet"></param>
    /// <returns></returns>
    public virtual GameEntity FindNearEnemy(bool onlyEnemyPlanet)
    {
        GameEntity enemy = null;
        var enemyPlanets = ownerPlanet.enemyPlanets;//敌对星球中寻找
        if (!onlyEnemyPlanet)
        {
            enemyPlanets = PlanetManager.Instance.allPlanets;//所有星球中寻找敌人
            
        }

        if (enemyPlanets.Count <= 0)
            return null;
        var planet = enemyPlanets[Random.Range(0, enemyPlanets.Count)];
        if (ownerPlanet.allyPlanets.Contains(planet))
        {
            return null;
        }
        if(planet)
        {
            if (planet.die)
                return null;

            if (isDefending && planet == defendingPlanet)//避开自己驻守的星球
                return null;

            if (planet == ownerPlanet)
                return null;
            
            
            if (planet.battleUnits.Count == 0)
            {
                if (inWar || Vector3.Distance(planet.transform.position, transform.position) < findEnemyDistance) //宣战状态
                {
                    enemy = planet;
                    return enemy;
                }
            
                return null;
            }
            
            // foreach (var enemyUnit in planet.battleUnits)
            // {
            //     if (enemyUnit == null)
            //     {
            //         continue;
            //     }
            //         
            //     if ((inWar ||Vector3.Distance(enemyUnit.transform.position, transform.position) < findEnemyDistance) &&
            //         enemyUnit.die == false && enemyUnit.GetVictimOwner() != GetAttackerOwner() )//随机从所有单位找一个，优先检测距离节省资源
            //     {
            //         enemy = enemyUnit;
            //         break;
            //     }
            // }
            
             float minDistance=100000;
             int minIndex = 0;
             for (int i=0;i< planet.battleUnits.Count;i++)
             {
                 var enemyUnit = planet.battleUnits[i];
                 if (enemyUnit == null || enemyUnit.die || enemyUnit.GetVictimOwner()==GetAttackerOwner())
                 {
                     continue;
                 }
            
                 var distance = Vector3.Distance(enemyUnit.transform.position, transform.position);
                 if (inWar || distance< findEnemyDistance)//随机从所有单位找一个，优先检测距离节省资源
                 {
                     if (distance < minDistance)
                     {
                         minDistance = distance;
                         minIndex = i;
                     }
                    
            
                 }
             }

             if (planet.battleUnits.Count == 1 && minIndex == 0)
                 return null;
            enemy = planet.battleUnits[minIndex];
            

        }
        
            
        return enemy;
    }

    public void SetChaseTarget(IVictimAble target)
    {
        this.chaseTarget = target;
    }

    public void ClaimWar()
    {
        if(canAttack)
            inWar = true;
    }

    public Planet GetAroundPlanet()
    {
        if (inWar && chaseTarget!=null && chaseTarget.GetVictimEntity().GetComponent<Planet>())
        {
            return chaseTarget as Planet;
        }
        if (isDefending)
            return defendingPlanet;
        return ownerPlanet;
    }

    public void WarOver()//战争结束，清除战争状态
    {
        inWar = false;
    }

    public void DefendPlanet()
    {
        if (isDefending && defendingPlanet)
        {
            defendingPlanet.Defend(GetAttackerOwner() as Planet,planetCommander.uid,Time.deltaTime);
        }
    }


    public void Recall()
    {
        CustomEvent.Trigger(gameObject, "OnDefendPlanetSet");
    }
    
    public void SetDefendTarget(Planet planet)
    {
        defendingPlanet = planet;
        CustomEvent.Trigger(gameObject, "OnDefendPlanetSet");
        isDefending = true;
        
    }
    public void ChangeOwnerPlanet(Planet planet)
    {
        ownerPlanet = planet;
        
    }

    public void Init(Planet planet,PlanetCommander planetCommander)
    {
        this.ownerPlanet = planet;
        moveManager.Init(planet);
        LogTip(planetCommander.uid+"");
        this.planetCommander = planetCommander;
       
        //isDefending = true;
        //defendingPlanet = planet;


    }

    public override void LogTip(string tip)
    {
        if (hpUI)
        {
            hpUI.LogTip(tip);
        }
        //Debug.Log(tip);
    }

    public override void Die()
    {
        base.Die();
        stateMachine.enabled = false;
        if (ownerPlanet)
        {
            ownerPlanet.battleUnits.Remove(this);
        }
        
        DieFx();
        //Destroy(gameObject);

        if (showHpUI && hpUI)
        {
            Destroy(hpUI.gameObject);
        }
        
        
        gameObject.SetActive(false);
        
    }
    
    
    
    public override void OnAttacked(AttackInfo attackInfo)
    {
        base.OnAttacked(attackInfo);
        if (!IsTargetAlive())//当自己处于和平状态时被袭击
        {
            if(Math.Abs(supportDistance) < 0.5f)
                return;
            if (attackInfo.attacker==null || attackInfo.attacker.GetAttackerOwner() == GetVictimOwner())//同一阵营
            {
                return;
                
            }
            if(ownerPlanet==null)
                return;
            for (int i = 0; i < ownerPlanet.battleUnits.Count; i++)
            {
                if (ownerPlanet.battleUnits[i]!=null && ownerPlanet.battleUnits[i]!=this && Vector3.Distance(ownerPlanet.battleUnits[i].transform.position, transform.position) < supportDistance)
                {
                    var supportAble = ownerPlanet.battleUnits[i].GetComponent<ISupportAble>();
                    supportAble?.Support(attackInfo.attacker as BattleUnit);
                }
            }
        }
       
    }


    public override GameEntity GetAttackerOwner()
    {
        return ownerPlanet;
    }

    public override GameEntity GetVictimOwner()
    {
        return ownerPlanet;
    }

   
}
