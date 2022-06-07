using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class McUnit : WarPlane
{
    public Vector3 targetMcPos;
    
    public float attackDistance;
    
    protected FightingManager fightingManager;

    //受击特效
    public SkinnedMeshRenderer[] meshRenderers;
    
    [Header("火焰特效")] public Transform fireFx;


    [Header("寻敌Trigger")] public SphereCollider trigger;
    [HideInInspector]public bool canPushBack=true;//是否可被击退,在箭塔时不可被击退哦

    protected override void Start()
    {
        base.Start();
        canPushBack = true;
       
        fightingManager = GameManager.Instance.fightingManager;
       
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
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
    /// <summary>
    /// 装备武器或者更换武器时调用,仅用于设置状态机相关判定数值，
    /// </summary>
    public void SetAttackDistance(float value)
    {
        this.attackDistance = value;
    }
    
    
    public bool IsInAttackRange()
    {
        if (chaseTarget == null)
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
    
    public override void GoMCPos(Vector3 pos,bool escape)
    {
        base.GoMCPos(pos,escape);

        this.targetMcPos = pos;
        CustomEvent.Trigger(gameObject, "OnDestinationSet",escape);
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

   

    public override void OnSlainOther()
    {
        base.OnSlainOther();
        if (planetCommander!=null)
        {
            planetCommander.AddPoint(2);
            fightingManager.AddPlayerDataValue(planetCommander.player.uid,"killCount",1);
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

    
}
