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

    //public int stoppingDistance=5;

    //附魔
    public int vampire = 0;//吸血
    public int fire = 0;//火焰附加
    public int parry = 0;//招架

    [Header("最大耐久")] public int maxEndurance=15;
    [Header("耐久")] public int endurance;

    private UnityEvent<IVictimAble,int> vampireDelegate;
    private UnityEvent fireDelegate;
    private UnityEvent parryDelegate;

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

    }

    public void AddEndurance(int value)
    {
        endurance += value;
        OnEnduranceChange(endurance,maxEndurance);
    }

    public virtual void Load(int endurance,int vampire,int fire,int parry)
    {
        this.endurance = endurance;
        this.vampire = vampire;
        this.fire = fire;
        this.parry = parry;
        OnSpellChange();
        OnEnduranceChange(endurance,maxEndurance);
    }

    public void SaveToCommander()
    {
        var steveCommander = owner.planetCommander as SteveCommander;
        if(steveCommander==null)
            return;
        steveCommander.weaponSaved = true;
        steveCommander.desireWeaponId = mcWeaponId;
        steveCommander.endurance = endurance;
        steveCommander.fire = fire;
        steveCommander.vampire = vampire;
        steveCommander.parry = parry;
    }

    //随机附魔
    public void RandomSpell()
    {
        
        var random0 = UnityEngine.Random.Range(0, 3);
        if (random0 == 0)
        {
            if(vampire==1)
                MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔失败");
            else
            {
                MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔-吸血");
            }
            vampire = 1;
        }

        if (random0 == 1)
        {
            if(fire==1)
                MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔失败");
            else
            {
                MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔-火焰");
            }
            fire = 1;
        }

        if (random0 == 2)
        {
            if(parry==1)
                MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔失败");
            else
            {
                MessageBox._instance.AddMessage("系统",owner.planetCommander.player.userName+"附魔-格挡");
            }
            parry = 1;
        }
            
            
        OnSpellChange();
            
    }
    
    public void OnSpellChange()
    {
        var str = "";
        if (vampire > 0)
        {
            if(owner.onAttackOther==null)
                owner.onAttackOther += OnAttackOther;
            str += "|吸血";
        }

        if (parry > 0)
        {
            if(owner.onAttacked==null)
                owner.onBeforeAttacked += Parry;
            str += "|格挡";
        }

        if (fire > 0)
        {
            if(owner.onAttackOther==null)
                owner.onAttackOther += OnAttackOther;
            str += "|火焰";
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
        endurance--;
        OnEnduranceChange(endurance,maxEndurance);
    }

    public void OnEnduranceChange(int endurance,int maxEndurance)
    {
        (owner as Steve).UpdateWeaponEndurance(endurance,maxEndurance);
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
        if (fire > 0)
        {
            if(victimAble.GetVictimEntity())
                SkillManager.Instance.AddSkill("Skill_着火_LV1",victimAble.GetVictimEntity(),owner.planetCommander);
        }
        if(vampire>0)
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,(int)(damage*0.25f)));
    }

    //招架
    public void OnAttacked(int damage)
    {
       
    }

    public AttackInfo Parry(AttackInfo attackInfo)
    {
        if(UnityEngine.Random.Range(0, 2) > 0)
        {
            attackInfo.value /= 2;
            Debug.Log("格挡");
        }

        return attackInfo;
    }


    private void OnDisable()
    {
        //清除所有事件绑定
        owner.onAttacked =null;
       
        owner.onAttackOther =null;
        owner.onBeforeAttacked = null;

    }
}
