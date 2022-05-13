using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    }

    //随机附魔
    public void RandomSpell()
    {
        
        var random0 = UnityEngine.Random.Range(0, 3);
        if (random0 ==0)
            vampire = 1;
        if (random0 ==1)
            fire = 1;
        if (random0 ==2)
            parry = 1;
            
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
                owner.onAttacked += OnAttacked;
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
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,2));
    }

    //招架
    public void OnAttacked(int damage)
    {
        if (parry>0 && UnityEngine.Random.Range(0, 2) > 0)
        {
            owner.OnAttacked(new AttackInfo(owner,AttackType.Heal,(int)(damage*0.5f)));
        }
    }


    private void OnDisable()
    {
        //清除所有事件绑定
        owner.onAttacked =null;
        owner.onAttackOther =null;
        
        
    }
}
