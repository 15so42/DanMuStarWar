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

    [Header("火焰特效")] public Transform fireFx;


    [Header("Trigger")] public SphereCollider trigger;
    [HideInInspector]public bool canPushBack=true;//是否可被击退,在箭塔时不可被击退哦

    private int curWeaponId;//切换武器时更新
   
    
    
    
    protected override void Start()
    {
        base.Start();
        canPushBack = true;
       
        fightingManager = GameManager.Instance.fightingManager;
       
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        //RandomWeapon();
        hpUI.SetNameText(planetCommander.player.userName);
        hpUI.OpenHPTile();
        

        if (planetCommander is SteveCommander commander)
        {
            commander.battleUnits.Add(this);
            if (commander.desireMaxHp != 0)
            {
                AddMaxHp(commander.desireMaxHp - props.maxHp);
            }
        }

        var liveWeapon = InitDesireWeapon();//读档，如果读取是空手就随机武器
        if (liveWeapon==null ||  (liveWeapon as HandWeapon).mcWeaponId == 0)//空手
        {
            RandomWeapon();
        }
        
        //todo 删除测试
        //ChangeWeapon(7);
        
        //关闭碰撞体
        //trigger.enabled = false;

    }

    public void OpenTrigger()
    {
        //.enabled = true;
    }

    public Weapon InitDesireWeapon()
    {
        var steveCommander = (planetCommander as SteveCommander);
        if (steveCommander == null)
            return null;
        
        var liveWeapon=ChangeWeapon(steveCommander.desireWeaponId);
        (liveWeapon as HandWeapon).Load(steveCommander.steveWeaponNbt);

        return liveWeapon; 
    }

    public void UpdateWeaponEndurance(int endurance,int maxEndurance)
    {
        if (hpUI && hpUI.gameObject)
        {
            hpUI.UpdateWeaponEndurance(endurance, maxEndurance);
        }
        
        if (endurance <= 0)
        {
            RandomWeapon();
        }
    }
    
    public void FixWeapon(int value)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        (liveWeapon as HandWeapon).AddEndurance(value);
    }

    public Weapon ChangeWeapon(int weaponId)
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        var liveWeapon = weapons.Find(x => (x as HandWeapon).mcWeaponId == weaponId);
        
        liveWeapon.gameObject.SetActive(true);
        liveWeapon.Init(this);
        hpUI.SetWeaponText(liveWeapon.weaponName);
        curWeaponId = weaponId;
        
        
        
        //(liveWeapon as HandWeapon)?.SaveToCommander();
        return liveWeapon;
    }

    public void RandomRareWeapon()
    {
        var random = UnityEngine.Random.Range(0, 2);
        if (curWeaponId == 7 || curWeaponId == 8)
        {
            if (curWeaponId == 7)
            {
                ChangeWeapon(8);
                return;
            }

            if (curWeaponId == 8)
            {
                ChangeWeapon(7);
                return;
            }
               
        }
        
        if(random==0)
            ChangeWeapon(7);
        if (random == 1)
            ChangeWeapon(8);
    }

    public void RandomWeapon()
    {
        if(die)
            return;
        
        Debug.Log("抽取武器");
        var steveCommander = (planetCommander as SteveCommander);
        if (steveCommander != null && steveCommander.point > 6)
        {
            
            var randomId=UnityEngine.Random.Range(1, weapons.Count - 2);
            var liveWeapon = ChangeWeapon(randomId);
            MessageBox._instance.AddMessage("[系统]" + planetCommander.player.userName + "抽取武器:" + liveWeapon.weaponName);
            
            hpUI.SetWeaponText(liveWeapon.weaponName);
            steveCommander.AddPoint(-6);

        }
        else
        {
            MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"自动抽取失败，点数不足");
            // var liveWeapon = ChangeWeapon(0);
            // hpUI.SetWeaponText(liveWeapon.weaponName);
        }
    }

    public bool TryRandomSpell(bool byGift)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        return (liveWeapon as HandWeapon).TryRandomSpell(byGift);
    }
    
    
    
    public void RandomSpell(bool rare,bool gift)
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        if ((liveWeapon as HandWeapon).weaponNbt.enhancementLevels.Count >= 3 && !gift)
        {
            (liveWeapon as HandWeapon).OnlyUpdateSpell();
        }
        else
        {
            (liveWeapon as HandWeapon).RandomSpell(rare);
        }
        
    }

    public void RemoveSpell()
    {
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);

        (liveWeapon as HandWeapon)?.RemoveSpell();
        
    }
    
    public Vector3 GetPos(int index)
    {
        return fightingManager.mcPosManager.GetPosByIndex(index);
    }

    public override void GoMCPos(Vector3 pos,bool escape)
    {
        base.GoMCPos(pos,escape);

        targetMcPos = pos;
        CustomEvent.Trigger(gameObject, "OnDestinationSet",escape);
    }


    public void OnBuyWeaponSuccess(string weaponName)
    {
        var targetWeapon = weapons.Find(x => x.weaponName == weaponName);
        var id = (targetWeapon as HandWeapon).mcWeaponId;
        ChangeWeapon(id);
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

    public override void AddMaxHp(int value)
    {
        base.AddMaxHp(value);
        (planetCommander as SteveCommander).desireMaxHp = props.maxHp;
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
    public void EnterDefendState(int findDistance,int attackDistance)
    {
        trigger.radius += findDistance;
        findEnemyDistance += findDistance;
        canPushBack = false;
        
        float minDistance = 10000;
        foreach (var w in weapons)
        {
            if(w.gameObject.activeSelf==false)
                continue;
            if (w.addAtkDistanceByDP)
            {
                w.attackDistance += attackDistance;
            }
            if (w.attackDistance < minDistance)
                minDistance = w.attackDistance;
            
        }
        
        SetAttackDistance(minDistance);
    }

    public void ExitDefendState(int findDistance,int attackDistance)
    {
        trigger.radius -= findDistance;
        findEnemyDistance -= findDistance;
        canPushBack = true;
        
        float minDistance = 10000;
        foreach (var w in weapons)
        {
            if(w.gameObject.activeSelf==false)
                continue;
            if (w.addAtkDistanceByDP)
            {
                w.attackDistance -= attackDistance;
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
        planetCommander.AddPoint(1);
        (planetCommander as SteveCommander).battleUnits.Remove(this);
        EventCenter.Broadcast(EnumEventType.OnSteveDied,this);
        
        //保存武器
        var liveWeapon = weapons.Find(x => x.gameObject.activeSelf);
        (liveWeapon as HandWeapon).SaveToCommander();
        
        base.Die();
        
       
    }
    
    public override void OnSlainOther()
    {
        base.OnSlainOther();
        planetCommander.AddPoint(2);
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
