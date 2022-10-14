using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using GameCode.Tools;
using UnityEngine;
using UnityEngine.AI;

public class McUnit : WarPlane
{
    public Vector3 targetMcPos;
    
    public float attackDistance;
    
    protected FightingManager fightingManager;

    //受击特效
    public SkinnedMeshRenderer[] meshRenderers;
    
    [Header("火焰特效")] public Transform fireFx;
    [Header("毒特效")] public Transform poisonFx;
    public Transform phoenixFx;
    public Transform angryFx;
    public Transform searingSunFx;


    [Header("寻敌Trigger")] public SphereCollider trigger;
    [HideInInspector]public bool canPushBack=true;//是否可被击退,在箭塔时不可被击退哦

    [Header("基地寻敌")] public bool canSetPlanetEnemy = true;

    [Header("驻守")] public bool isGuard;
    public Vector3 guardPos;
    [Header("驻守Trigger")]
    public GameObject dieRangePfb;

    private SteveGuardRange dieRangeGo;
    
    

    protected override void Start()
    {
        base.Start();
        canPushBack = true;
       
        fightingManager = GameManager.Instance.fightingManager;
       
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        LoadFx();
        OnHpChanged(props.hp,props.maxHp,props.shield,props.maxShield);
    }

    void LoadFx()
    {
        var position = transform.position;
        angryFx = ResFactory.Instance.CreateFx("AngryFx", position).transform;
        poisonFx = ResFactory.Instance.CreateFx("PoisonFx", position).transform;
        fireFx = ResFactory.Instance.CreateFx("FireFx", position).transform;
        searingSunFx = ResFactory.Instance.CreateFx("SearingSunFx", position).transform;
        phoenixFx = ResFactory.Instance.CreateFx("PhoenixFx", position).transform;

        angryFx.SetParent(transform);
        poisonFx.SetParent(transform);
        fireFx.SetParent(transform);
        searingSunFx.SetParent(transform);
        phoenixFx.SetParent(transform);
        
        angryFx.gameObject.SetActive(false);
        poisonFx.gameObject.SetActive(false);
        fireFx.gameObject.SetActive(false);
        searingSunFx.gameObject.SetActive(false);
        phoenixFx.gameObject.SetActive(false);
        
    }
    
    public Vector3 RandomDestination()
    {
        Vector3 newDest = UnityEngine.Random.insideUnitSphere * 25 ; //半径为500的球形随机点
        NavMeshHit hit;
        bool hasDestination = NavMesh.SamplePosition(newDest, out hit, 100,1); //unity到指定点最接近的位置
       
        return newDest;
    }

    public bool SelfOutGuardRange()
    {
        if (Vector3.Distance(transform.position , guardPos) >findEnemyDistance)
        {
            return true;
        }

        return false;
    }

    public bool InGuardRange()
    {
        
        if (chaseTarget == null|| chaseTarget.GetVictimEntity()==null)
            return false;
        if (Vector3.Distance(chaseTarget.GetVictimEntity().transform.position , guardPos) <findEnemyDistance)
        {
            return true;
        }

        return false;
    }

    public bool NearTargetPos()
    {
        float distance = Vector3.Distance(transform.position, moveManager.tmpTarget);
        return distance < 1f;
    }
    
    public HandWeapon GetActiveWeapon()
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        return liveWeapon as HandWeapon;
    }

    public virtual void SetGuardStats(bool status)
    {
        isGuard = status;
        if (status )
        {
            if(dieRangeGo!=null)
                Destroy(dieRangeGo.gameObject);
            dieRangeGo = Instantiate(dieRangePfb).GetComponent<SteveGuardRange>();
            dieRangeGo.Init(this,trigger.radius);
            dieRangeGo.transform.position = transform.position;
        }
        else
        {
            if(dieRangeGo!=null)
                Destroy(dieRangeGo.gameObject);
        }
       
    }
    
    public virtual GameEntity OverLapEnemyInMc()
    {
        var colliders = Physics.OverlapSphere(transform.position, trigger.radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            var victim= EnemyCheck(colliders[i]);
            if (victim != null && victim.GetVictimEntity()!=null)
                return victim.GetVictimEntity();
            continue;
            
            
            
            
            var collider1 = colliders[i];
            var gameEntity = collider1.GetComponent<GameEntity>();
            if (!gameEntity) //不是单位
                continue;

            var gameEntityOwner = gameEntity.GetVictimOwner();
            if (gameEntityOwner == GetAttackerOwner()) //同星球
                continue;
            
            if (gameEntity.die) //已经死亡
                continue;

            // var targetPlanet = gameEntityOwner as Planet;
            // if (targetPlanet == null) //如果只对敌对星球寻敌，而敌对星球不存在，或找到的单位不属于，不算作敌人
            //     return null;
            
            return gameEntity;
        }

        return null;
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
    /// <summary>
    /// 装备武器或者更换武器时调用,仅用于设置状态机相关判定数值，
    /// </summary>
    public void SetAttackDistance(float value)
    {
        this.attackDistance = value;
    }
    
    
    public bool IsInAttackRange()
    {
        if (chaseTarget == null || chaseTarget.GetVictimEntity()==null)
            return false;
        float distance = Vector3.Distance(chaseTarget.GetVictimEntity().transform.position, transform.position);
        if (distance < attackDistance)
            return true;
        return false;
    }
    

    #region 耐久
    public void UpdateWeaponEndurance(int endurance,int maxEndurance)
    {
        if (hpUI && hpUI.gameObject)
        {
            hpUI.UpdateWeaponEndurance(endurance, maxEndurance);
        }
        
        if (endurance <= 0)
        {
            //RandomWeapon();
        }
    }
    

    #endregion

    public Vector3 GetEnemyPlanetPos()
    {

        Vector3 enemyPlanetPos = Vector3.zero;
        if ( ownerPlanet.enemyPlanets.Count>0)
        {
            enemyPlanetPos=ownerPlanet.enemyPlanets[0].transform.position;
        }

        return enemyPlanetPos;

    }
    
    public Vector3 GetPos(int index)
    {
        return fightingManager.mcPosManager.GetPosByIndex(index);
    }
    
    public override void GoMCWorldPos(Vector3 pos,bool escape)
    {
        base.GoMCWorldPos(pos,escape);

        this.targetMcPos = pos;
        CustomEvent.Trigger(gameObject, "OnDestinationSet",escape);
    }


    public IVictimAble EnemyCheck(IVictimAble victim)
    {
        var gameEntity = victim.GetVictimEntity();
        if (!gameEntity)//不是单位
            return null;

        var gameEntityOwner = gameEntity.GetVictimOwner();
        if (gameEntity==null || gameEntityOwner == GetAttackerOwner()) //同星球
            return null;
        if (gameEntity.die)//已经死亡
            return null;

        var targetPlanet = gameEntityOwner as Planet;
        if(targetPlanet==null )//只要不是自己星球的，都算作敌人
            return gameEntity;
        
        if (ownerPlanet==null || ownerPlanet.enemyPlanets.Contains(targetPlanet) )
        {
            return gameEntity;
        }

        return null;
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
        if(targetPlanet==null )//只要不是自己星球的，都算作敌人
            return gameEntity;
        
        if (ownerPlanet==null || ownerPlanet.enemyPlanets.Contains(targetPlanet) )
        {
            return gameEntity;
        }

        return null;
        
    }

    

    /// <summary>
    /// 受击特效
    /// </summary>
    /// <param name="attackInfo"></param>
    public override BattleUnitProps.HpAndShield OnAttacked(AttackInfo attackInfo)
    {
        var hpAndShield = base.OnAttacked(attackInfo);
        
        if(attackInfo.attackType==AttackType.Heal)
            return hpAndShield;
        
        StopAllCoroutines();
        if (gameObject.activeSelf)
        {
            StartCoroutine(VictimFx());
        }

        return hpAndShield;
    }

   

    public override void OnSlainOther(GameEntity victim)
    {
        base.OnSlainOther(victim);
        if (planetCommander!=null)
        {
            planetCommander.AddPoint(2);
            ownerPlanet.OnAttacked(new AttackInfo(this, AttackType.Heal, 5));
            if (victim as Steve)
            {
                fightingManager.AddPlayerDataValue(planetCommander.player.uid,"killCount",1);
            }

            if (victim as McPlanetTower)
            {
                planetCommander.AddPoint(3);
            }
            
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

   

    public void OpenTrigger()
    {
        trigger.gameObject.SetActive(true);
    }
    
    //
    public void OpenFireFx()
    {
        fireFx.gameObject.SetActive(true);
    }

    public void CloseFireFx()
    {
        fireFx.gameObject.SetActive(false);
    }

    public void OpenPhoenixFx()
    {
        if(phoenixFx &&!phoenixFx.gameObject.activeSelf)
            phoenixFx.gameObject.SetActive(true);
    }

    public void ClosePhoenixFx()
    {
        if(phoenixFx && phoenixFx.gameObject.activeSelf)
            phoenixFx.gameObject.SetActive(false);
    }

    public void OpenAngryFx()
    {
        if(angryFx && !angryFx.gameObject.activeSelf)
            angryFx.gameObject.SetActive(true);
    }

    public void CloseAngryFx()
    {
        if(angryFx && angryFx.gameObject.activeSelf)
            angryFx.gameObject.SetActive(false);
    }

    public void OpenSunFx()
    {
        if(searingSunFx && !searingSunFx.gameObject.activeSelf)
            searingSunFx.gameObject.SetActive(true);
    }

    public void CloseSunFx()
    {
        if(searingSunFx && searingSunFx.gameObject.activeSelf)
            searingSunFx.gameObject.SetActive(false);
    }

    public void OpenDrawFx()
    {
        
    }

    public void CloseDrawFx()
    {
        
    }

    public void OpenPoisonFx()
    {
        poisonFx.gameObject.SetActive(true);
    }
    
    public void ClosePoisonFx()
    {
        poisonFx.gameObject.SetActive(false);
    }


    public override void Die()
    {
        base.Die();
        EventCenter.Broadcast(EnumEventType.OnMcUnitDied,this);
        
    }
}
