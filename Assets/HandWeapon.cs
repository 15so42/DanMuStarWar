using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = System.Random;

[Serializable]
public class HandWeapon : Weapon
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
    

    public override void Init(BattleUnit owner)
    {
        base.Init(owner);
        root = transform.root;
        animator = root.GetComponent<BattleUnit>().animator;
        if(animator==null)
            Debug.Log(name+"需要手动配置");
        (owner as Steve).SetAttackDistance(attackDistance);
        
        endurance = maxEndurance;
        OnEnduranceChange(endurance,maxEndurance);

        weaponNbt=new SteveWeaponNbt();
        
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
        if(steveCommander==null)
            return;
        
        
        steveCommander.steveWeaponNbt = this.weaponNbt;
        steveCommander.desireWeaponId = mcWeaponId;
        
    }

    public bool TryRandomSpell(bool byGift)
    {
        if (weaponNbt.enhancementLevels.Count >= 3 && !byGift)
        {
            MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔已达上限，投喂打call可继续附魔");

            return false;
        }

        if (weaponNbt.enhancementLevels.Count >= randomStrs.Count )
        {
            MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"已获得所有附魔,无法再附魔了");
            return false;
        }

        return true;
    }

    //随机附魔
    public void RandomSpell(bool rare)
    {

        
        var spellStr = randomStrs[UnityEngine.Random.Range(0, randomStrs.Count)];
        if (GetWeaponLevelByNbt(spellStr) > 0)
        {
            MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔失败");
        }
        else
        {
            MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔"+spellStr);
            SetWeaponLevel(spellStr, 1);
        }
            
            
        OnSpellChange();
            
    }
    
    public void OnSpellChange()
    {
        var str = "";
        for (int i = 0; i < weaponNbt.enhancementLevels.Count; i++)
        {
            if (weaponNbt.enhancementLevels[i].level>0)
            {
                str += "|" + weaponNbt.enhancementLevels[i].enhancementName;
            }
        }
        
        owner.hpUI.SetWeaponText(weaponName+str);
    }


    public override bool FireCheck()
    {
        var distance =
            Vector3.Distance(root.transform.position, owner.chaseTarget.GetVictimEntity().transform.position);
        if (distance < attackDistance)
        {
            return true;
        }

        return false;
    }

    public override void Fire()
    {
        animator.SetTrigger("Attack");
        Invoke(nameof(Damage),0.3f);
        if (GetWeaponLevelByNbt("耐久") > 0)
        {
            var random=UnityEngine.Random.Range(0, 3);
            if (random == 0)
            {
                //不扣除耐久
            }
            else
            {
                endurance--;
                OnEnduranceChange(endurance,maxEndurance);
            }
        }
        else
        {
            endurance--;
            OnEnduranceChange(endurance,maxEndurance);
        }
        
    }

    public void OnEnduranceChange(int endurance,int maxEndurance)
    {
        (owner as Steve).UpdateWeaponEndurance(endurance,maxEndurance);
        weaponNbt.endurance = endurance;
    }

    public void Damage()
    {
        if(owner.chaseTarget==null)
            return;
        var victim = owner.chaseTarget.GetVictimEntity();
        victim.OnAttacked(new AttackInfo(this.owner,AttackType.Physics,attackValue));
        var navMeshMoveManager = victim.GetComponent<NavMeshMoveManager>();
        if(navMeshMoveManager)
            navMeshMoveManager.PushBack(victim.transform.position-transform.position+Vector3.up*pushBackHeight,pushBackStrength);
        
    }


    //吸血
    public void OnAttackOther(IVictimAble victimAble,int damage)
    {
        if (GetWeaponLevelByNbt("火焰") > 0)
        {
            if(victimAble.GetVictimEntity())
                SkillManager.Instance.AddSkill("Skill_着火_LV1",victimAble.GetVictimEntity(),owner.planetCommander);
        }
        if(GetWeaponLevelByNbt("吸血")>0)
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,(int)(damage*0.25f)));

        
    }

    //招架
    public void OnAttacked(int damage)
    {
       
    }

    public void OnSlainOther()
    {
        if (GetWeaponLevelByNbt("凯旋")>0)
        {
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,10));    
        }

        if (GetWeaponLevelByNbt("经验修补") > 0)
        {
            AddEndurance(maxEndurance/3);
        }
    }

    public AttackInfo OnBeforeAttacked(AttackInfo attackInfo)
    {
        if (GetWeaponLevelByNbt("格挡") > 0)
        {
            if(UnityEngine.Random.Range(0, 2) > 0)
            {
                attackInfo.value /= 2;
                Debug.Log("格挡");
            }
            
        }
        if (GetWeaponLevelByNbt("锋利") > 0)
        {
            attackInfo.value=(int)(attackInfo.value*1.25f);
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
       
        owner.onAttackOther -= OnAttackOther ;
        owner.onBeforeAttacked -= OnBeforeAttacked;

    }
}
