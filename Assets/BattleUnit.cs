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


    private bool awaked = false;
    protected void Awake()
    {
        base.Awake();
        moveManager = GetComponent<MoveManager>();
       
        
        
        gameManager=GameManager.Instance;
        planetManager = gameManager.planetManager;
        battleUnitManager = gameManager.battleUnitManager;
        animator = GetComponentInChildren<Animator>();

        awaked = true;

    }

    public bool IsTargetAlive()
    {
        var target = chaseTarget?.GetVictimEntity();
        return chaseTarget != null && target.IsAlive() ;
    }

    public bool IsInFindRange()
    {
        var target = chaseTarget?.GetVictimEntity();
        var ret=target != null  && !target.die && Vector3.Distance(target.transform.position,transform.position)<findEnemyDistance;
        if (ret == false)
        {
            // if (target == null)
            // {
            //     Debug.Log("nullInRange");
            // }
            // else
            // {
            //     Debug.Log(target+""+target.die+Vector3.Distance(target.transform.position,transform.position));
            // }
            
        }
        return ret;
        //return target != null  && !target.die && Vector3.Distance(target.transform.position,transform.position)<findEnemyDistance;
    }

    
    /// <summary>
    /// MC模式表示前往某地
    /// </summary>
    public virtual void GoMCPos(Vector3 pos,bool escape)
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


   
    

    //星球大战模式专用，整个场景只有单位是碰撞体，因此不会漏寻敌，但是MC模式会漏寻敌，因为可能随机到场景的碰撞体
    public virtual GameEntity OverLapEnemy(bool onlyEnemyPlanet=true)
    {
        var colliders = Physics.OverlapSphere(transform.position, findEnemyDistance);
        //for (int i=0; i < colliders.Length;i++)
        //{
            var collider1 = colliders[Random.Range(0,colliders.Length)];
            var gameEntity = collider1.GetComponent<GameEntity>();
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
            
            if (onlyEnemyPlanet && ownerPlanet.enemyPlanets.Contains(targetPlanet))
            {
                return gameEntity;
            }

            if (!onlyEnemyPlanet)
            {
                return gameEntity;
            }
            

            

        //}

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

             //if (planet.battleUnits.Count == 1 && minIndex == 0)
             //    return null;
             enemy = planet.battleUnits[minIndex];
            

        }
        
            
        return enemy;
    }

    public void SetChaseTarget(IVictimAble target)
    {
        if(target.GetVictimEntity().canBeTarget==false)
            return;
        
        if (IsTargetAlive() == false)
        {
            this.chaseTarget = target;
            return;
        }
        
        
        var position = transform.position;
        float oldDistance = (chaseTarget.GetVictimEntity().transform.position - position).sqrMagnitude;
        float newDistance = (target.GetVictimEntity().transform.position - position).sqrMagnitude;

        if (newDistance < oldDistance)
        {
            this.chaseTarget = target;
        }
        
        
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
        if (!awaked)
        {
            Awake();
            awaked = true;
        }
           
        
        moveManager.Init(planet);
        //LogTip(planetCommander.uid+"");
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
        if (die)
        {
           return;
        }
        OnDieCount();
        base.Die();
        stateMachine.enabled = false;
        if (ownerPlanet)
        {
            ownerPlanet.battleUnits.Remove(this);
        }

        lastAttacker?.GetAttackEntity().OnSlainOther();
        DieFx();
        //Destroy(gameObject);

        if (showHpUI && hpUI)
        {
            Destroy(hpUI.gameObject);
        }
        
        
        gameObject.SetActive(false);
        
    }

    public virtual void AddMaxHp(int value)
    {
        props.AddMaxHp(value);
        onHpChanged.Invoke(props.hp,props.maxHp,props.shield,props.maxShield);
    }
    
    
    public override BattleUnitProps.HpAndShield OnAttacked(AttackInfo attackInfo)
    {
        if (attackInfo.attackType == AttackType.Heal)
        {
            OnHealSelfCount(attackInfo.value);
        }
        else
        {
            OnAttackedCount(attackInfo.value);
        }
        
        var hpAndShield = base.OnAttacked(attackInfo);
        
        
        
        
        if (!IsTargetAlive())//当自己处于和平状态时被袭击
        {
            if (Math.Abs(supportDistance) < 0.5f)
                return hpAndShield;
            if (attackInfo.attacker==null || attackInfo.attacker.GetAttackerOwner() == GetVictimOwner())//同一阵营
            {
                return hpAndShield;
                
            }
            if(ownerPlanet==null)
                return hpAndShield;
            for (int i = 0; i < ownerPlanet.battleUnits.Count; i++)
            {
                if (ownerPlanet.battleUnits[i]!=null && ownerPlanet.battleUnits[i]!=this && Vector3.Distance(ownerPlanet.battleUnits[i].transform.position, transform.position) < supportDistance)
                {
                    var supportAble = ownerPlanet.battleUnits[i].GetComponent<ISupportAble>();
                    supportAble?.Support(attackInfo.attacker as BattleUnit);
                }
            }
        }

        return hpAndShield;

    }

    public override void OnAttackOther(IVictimAble victimAble, AttackInfo attackInfo)
    {
        if (attackInfo.attackType == AttackType.Heal)
        {
            OnHealOtherCount(attackInfo.value);
        }
        else
        {
            OnAttackOtherCount(attackInfo.value);
        }
        base.OnAttackOther(victimAble, attackInfo);
    }


    public override GameEntity GetAttackerOwner()
    {
        return ownerPlanet;
    }

    public override GameEntity GetVictimOwner()
    {
        return ownerPlanet;
    }

    public override void OnSlainOther()
    {
        base.OnSlainOther();
        OnSlainOtherCount();
    }
   
    
    //伤害统计
    public void OnAttackOtherCount(int damage)
    {
        if(planetCommander==null)
            return;
        planetCommander.attackOtherDamage += damage;
    }
    
    public void OnAttackedCount(int damage)
    {
        if(planetCommander==null)
            return;
        planetCommander.attackedDamage += damage;
    }

    public void OnHealSelfCount(int value)
    {
        if(planetCommander==null)
            return;
        planetCommander.healSelfValue+=value;
    }
    
    public void OnHealOtherCount(int value)
    {
        if(planetCommander==null)
            return;
        planetCommander.healOtherValue+=value;
    }
    
    public void OnSlainOtherCount()
    {
        if(planetCommander==null)
            return;
        planetCommander.slainCount++;
    }

    public void OnDieCount()
    {
        if(planetCommander==null)
            return;
        planetCommander.dieCount++;
    }
    
    
    
}
