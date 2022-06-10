using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = System.Random;

[Serializable]
public class HandWeapon : Weapon,IDamageAble
{
    [Header("武器id")] public int mcWeaponId;
    
    private Transform root;
    [HideInInspector] public Animator animator;
    
    
    //附魔用nbt代替
    public SteveWeaponNbt weaponNbt;

    [Header("附魔列表")] public List<string> randomStrs = new List<string>();
    [Header("高阶附魔")] public List<string> rareRandomStrs=new List<string>();

    [Header("最大耐久")] public int maxEndurance=15;
    [Header("耐久")] public int endurance;
    

    [Header("击飞高度和力度")]
    public int pushBackHeight=4;
    public int pushBackStrength=1;


    public void SetMaxSpellCount(int value)
    {
        weaponNbt.maxSpellCount = value;
    }
    
    public override void Init(BattleUnit owner)
    {
        base.Init(owner);
        root = transform.root;
        animator = root.GetComponent<BattleUnit>().animator;
        if(animator==null)
            Debug.Log(name+"需要手动配置");
        (owner as McUnit).SetAttackDistance(attackDistance);
        
        weaponNbt=new SteveWeaponNbt();
        
        endurance = maxEndurance;
        OnEnduranceChange(endurance,maxEndurance);

        
        
        AddEvent();
    }

    void AddEvent()
    {
        
        owner.onAttackOther += OnAttackOther;
        
        owner.onBeforeAttacked += OnBeforeAttacked;

        owner.onAttacked += OnAttacked;
        owner.onSlainOther += OnSlainOther;

    }

    public void AddEndurance(int value)
    {
        endurance += value;
        OnEnduranceChange(endurance,maxEndurance);
    }

    public virtual void Load(SteveWeaponNbt weaponNbt)
    {
        if(weaponNbt==null)//没有nbt数据
            return;
        this.endurance = weaponNbt.endurance;
        this.weaponNbt = weaponNbt;
        
        OnSpellChange();
        OnEnduranceChange(endurance,maxEndurance);
        
    }

  

    public void SaveToCommander()
    {
        var steveCommander = owner.planetCommander as SteveCommander;
        if (steveCommander == null)
        {
            Debug.Log("steveCimmander为空");
            return;
        }
           
        
        steveCommander.steveWeaponNbt = this.weaponNbt;
        steveCommander.desireWeaponId = mcWeaponId;
        
    }

    public bool TryRandomSpell(bool byGift)
    {
        // if (weaponNbt.enhancementLevels.Count >= 3 && !byGift)
        // {
        //     MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔已达上限，投喂打call可继续附魔");
        //
        //     return false;
        // }

        // if (weaponNbt.enhancementLevels.Count >= randomStrs.Count )
        // {
        //     //MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"已获得所有附魔,无法再附魔了");
        //     //return false;
        // }

        return true;
    }

    public bool TrySpecificSpell(string name)
    {
        if (randomStrs.Contains(name) == false)
        {
            var str = "";
            foreach (var t in randomStrs)
            {
                str += " "+ t;
            }
            MessageBox._instance.AddMessage(owner.planetCommander.player.userName,"附魔名称错误，可附魔列表为："+str);
            return false;
        }
        
        if ((owner.planetCommander as SteveCommander).leftSpecificSpell <= 0)
        {
            MessageBox._instance.AddMessage(owner.planetCommander.player.userName,"指定附魔次数已用完");
            return false;
        }
            
        if (weaponNbt.enhancementLevels.Find(x => x.enhancementName == name) != null)
        {
            return true;
        }
        //新附魔
        if (weaponNbt.enhancementLevels.Count >= weaponNbt.maxSpellCount)
        {
            MessageBox._instance.AddMessage(owner.planetCommander.player.userName,"达到附魔栏位限制");
            return false;
        }
        // if (weaponNbt.enhancementLevels.Count >= weaponNbt.maxSpellCount)
        // {
        //     owner.LogTip("到达附魔上限，投喂打call可增加附魔栏位");
        //     return false;
        // }

        

        return true;
    }

    public void SpecificSpell(string name)
    {
        SetWeaponLevel(name,GetWeaponLevelByNbt(name)+1);
        OnSpellChange();
    }

    public void OnlyUpdateSpell()
    {
        var randomEnhancementLevel =  weaponNbt.enhancementLevels[UnityEngine.Random.Range( 0,weaponNbt.enhancementLevels.Count)];
        randomEnhancementLevel.level++;
        OnSpellChange();
    }
    
    //随机附魔
    public void RandomSpell(bool rare)
    {

        
        var spellStr = randomStrs[UnityEngine.Random.Range(0, randomStrs.Count)];
       
       
        MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔"+spellStr);
        SetWeaponLevel(spellStr, GetWeaponLevelByNbt(spellStr)+1);
            
            
        OnSpellChange();
            
    }

    public bool RemoveSpell(int index)
    {
        if (index - 1 < 0 || index - 1 >= weaponNbt.enhancementLevels.Count || weaponNbt.enhancementLevels.Count==0)
        {
            owner.planetCommander.commanderUi.LogTip("序号错误");
            return false;
        }

        
        weaponNbt.enhancementLevels.RemoveAt(index-1);
        OnSpellChange();
        return true;

    }
    
    public void OnSpellChange()
    {
        var str = "";
        for (int i = 0; i < weaponNbt.enhancementLevels.Count; i++)
        {
            if (weaponNbt.enhancementLevels[i].level>0)
            {
                str += "|" + weaponNbt.enhancementLevels[i].enhancementName+weaponNbt.enhancementLevels[i].level;
            }
        }
        
        owner.hpUI.SetWeaponText(weaponName+str);
    }


    public override bool FireCheck()
    {
        if (endurance <= 0)
        {
            return false;
        }
        
        var distance =
            Vector3.Distance(root.transform.position, owner.chaseTarget.GetVictimEntity().transform.position);
        if (distance < attackDistance)
        {
            return true;
        }

        return false;
    }

    public virtual void FireAnim()
    {
        animator.SetTrigger("Attack");
        
        Invoke(nameof(Damage),0.3f);
    }

    public override void Fire()
    {
        FireAnim();

        // var enduranceLevel = GetWeaponLevelByNbt("耐久");
        // if (enduranceLevel > 0)
        // {
        //     var random=UnityEngine.Random.Range(0, 100);
        //     if (random < (0.2+0.1*enduranceLevel)*100)
        //     {
        //         //不扣除耐久
        //     }
        //     else
        //     {
        //         endurance--;
        //         OnEnduranceChange(endurance,maxEndurance);
        //     }
        // }
        // else
        // {
        //     endurance--;
        //     OnEnduranceChange(endurance,maxEndurance);
        // }
        endurance--;
        OnEnduranceChange(endurance,maxEndurance);
    
    }

    public void OnEnduranceChange(int endurance,int maxEndurance)
    {
        (owner as McUnit).UpdateWeaponEndurance(endurance,maxEndurance);
        weaponNbt.endurance = endurance;
    }

    private float spellTimer = 0;
    void Update()
    {
        spellTimer += Time.deltaTime;
        
        if (spellTimer > 5)
        {
            var enduranceLevel = GetWeaponLevelByNbt("耐久");
            if (enduranceLevel >0)
            {
                endurance += 1 * enduranceLevel;
                if (endurance >= maxEndurance)
                {
                    (owner.planetCommander as SteveCommander).AddPoint(0.05f*2);
                }
                
            }
            spellTimer = 0;
        }
    }
    

    public void Damage()
    {
        if(owner.chaseTarget==null)
            return;
        var victim = owner.chaseTarget.GetVictimEntity();

        var attackInfo = new AttackInfo(this.owner, AttackType.Physics, attackValue);
        var sharpLevel = GetWeaponLevelByNbt("锋利");
        if (sharpLevel > 0)
        {
            attackInfo.value=(int)(attackInfo.value*(1+ (0.25f+sharpLevel*0.1f)));
        }
        
        var criticalLevel=GetWeaponLevelByNbt("暴击");
        if (criticalLevel > 0)
        {
            var rate = UnityEngine.Random.Range(0, 10);
            if (rate < criticalLevel)
            {
                attackInfo.value = (int) (attackInfo.value * (1.5 + criticalLevel * 0.1));
            }
        }

        
        var realDamage= victim.OnAttacked(attackInfo);
        var navMeshMoveManager = victim.GetComponent<NavMeshMoveManager>();
        // if(navMeshMoveManager)
        //     navMeshMoveManager.PushBack(victim.transform.position-transform.position+Vector3.up*pushBackHeight,pushBackStrength);
        if(navMeshMoveManager)
            navMeshMoveManager.PushBackByPos(victim.transform.position,transform.position,pushBackHeight,pushBackStrength);
        
        
        
        OnDamageOther(victim,realDamage);
    }


    //吸血
    public void OnAttackOther(IVictimAble victimAble,int damage)
    {
        // if (gameObject.activeSelf == false)
        //     return;
        //
        // var fireLevel = GetWeaponLevelByNbt("火焰");
        // if (fireLevel > 0)
        // {
        //     if (victimAble.GetVictimEntity())
        //     {
        //         var skill=SkillManager.Instance.AddSkill("Skill_着火_LV1", victimAble.GetVictimEntity(), owner.planetCommander);
        //         if (skill as FireSkill)//第一次附加火焰没问题，但是之后无法再附加火焰而是刷新火焰Buff
        //         {
        //             (skill as FireSkill).SetAttacker(owner); 
        //             (skill as FireSkill).life = 4 + fireLevel;
        //         }
        //         
        //     }
        //     
        // }
        //
        // var vampireLevel = GetWeaponLevelByNbt("吸血");
        //
        // if (vampireLevel > 0)
        // {
        //     owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,(int)(damage* (0.2f+0.05*fireLevel)  )));
        // }
        //     

        
    }
    
    
    //通过武器造成伤害
    public GameEntity GetVictimOwner()
    {
        return owner;
    }

    public GameEntity GetVictimEntity()
    {
        return owner;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
    

    public void OnDamageOther(IVictimAble victimAble, BattleUnitProps.HpAndShield realDamage)
    {
        if (gameObject.activeSelf == false)
            return;
        
        var fireLevel = GetWeaponLevelByNbt("火焰");
        if (fireLevel > 0)
        {
            if (victimAble.GetVictimEntity())
            {
                var skill=SkillManager.Instance.AddSkill("Skill_着火_LV1", victimAble.GetVictimEntity(), owner.planetCommander);
                if (skill as FireSkill)//第一次附加火焰没问题，但是之后无法再附加火焰而是刷新火焰Buff
                {
                    (skill as FireSkill).SetAttacker(owner); 
                    (skill as FireSkill).life = 4 + fireLevel*2;
                }
                
            }
            
        }

        var vampireLevel = GetWeaponLevelByNbt("吸血");

        if (vampireLevel > 0)
        {
            
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,Mathf.CeilToInt(realDamage.calAttackInfo.value * (0.2f+0.1f*fireLevel))));
        }
    }

    //招架
    public void OnAttacked(AttackInfo attackInfo)
    {
        if (gameObject.activeSelf == false)
            return;
        if (attackInfo.attackType == AttackType.Heal)
        {
            return;
        }
        
        // var thronLevel = GetWeaponLevelByNbt("荆棘");
        // if (thronLevel > 0 && attackInfo.attackType!=AttackType.Reflect)
        // {
        //     attackInfo.attacker.GetAttackEntity()
        //         .OnAttacked(new AttackInfo(owner, AttackType.Reflect, Mathf.CeilToInt(attackInfo.value * (0.15f + thronLevel*0.05f))));
        // }
    }

    public void OnSlainOther()
    {
        if (gameObject.activeSelf == false)
            return;
        var triumphLevel = GetWeaponLevelByNbt("凯旋");
        if (triumphLevel>0)
        {
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,(int)(4+triumphLevel*3)));    
        }

        var expFixLevel = GetWeaponLevelByNbt("经验修补");
        if (expFixLevel > 0)
        {
            AddEndurance((int)(maxEndurance*(0.15f+0.05f*expFixLevel)));
        }
    }

    public AttackInfo OnBeforeAttacked(AttackInfo attackInfo)
    {
        if (gameObject.activeSelf == false)
            return attackInfo;
        
        var thronLevel = GetWeaponLevelByNbt("荆棘");
        if (thronLevel > 0 && attackInfo.attackType!=AttackType.Reflect && attackInfo.attackType!=AttackType.Heal)
        {
            attackInfo.attacker.GetAttackEntity()
                .OnAttacked(new AttackInfo(owner, AttackType.Reflect, Mathf.CeilToInt(attackInfo.value * (0.15f + thronLevel*0.1f))));
        }
        
        var parryLevel = GetWeaponLevelByNbt("格挡");
        if (parryLevel > 0 && attackInfo.attackType!=AttackType.Heal)
        {
            var targetValue = (0.15 + 0.1 * parryLevel)*100;
            if(UnityEngine.Random.Range(0, 100) <targetValue)
            {
                attackInfo.value /= 2;
                Debug.Log("格挡");
            }
        }

        
        return attackInfo;
    }

    public int GetWeaponLevelByNbt(string key)
    {
        EnhancementLevel ret = null;
        ret = weaponNbt.enhancementLevels.Find(x => x.enhancementName == key);
        if (ret == null)
            return 0;
        else
        {
            return ret.level;
        }
    }

    public void SetWeaponLevel(string key, int level)
    {
        EnhancementLevel ret = null;
        ret = weaponNbt.enhancementLevels.Find(x => x.enhancementName == key);
        if (ret == null)
        {
            weaponNbt.enhancementLevels.Add(new EnhancementLevel(key,level));
            
            return;
        }
        else
        {
            ret.level = level;
        }
    }


    private void OnDisable()
    {
        //清除所有事件绑定
        owner.onAttacked -= OnAttacked;

        owner.onSlainOther -= OnSlainOther;
        owner.onAttackOther -= OnAttackOther ;
        owner.onBeforeAttacked -= OnBeforeAttacked;

    }
}
