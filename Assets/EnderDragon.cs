using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnderDragon : McUnit
{

    private int lastHp;
    private HandWeapon liveWeapon;
    protected override void Start()
    {
        
        base.Start();
        liveWeapon = GetComponentInChildren<HandWeapon>();
        liveWeapon.Init(this);
        liveWeapon.randomStrs.Remove("烈阳");
        liveWeapon.randomStrs.Remove("坚韧");
        liveWeapon.immortalCd = 13;
        
        canPushBack = false;
        
        onHpChanged += UpdateHpUIByNameText;
        UpdateHpUIByNameText(props.hp,props.maxHp,props.shield,props.maxShield);
        lastHp = props.maxHp;

        //liveWeapon.TrySpecificSpell("龙鳞");
        liveWeapon.weaponNbt.enhancementLevels.Add(new EnhancementLevel("龙鳞",1));
        liveWeapon.weaponNbt.enhancementLevels.Add(new EnhancementLevel("保护",1));
        //liveWeapon.weaponNbt.enhancementLevels.Add(new EnhancementLevel("不灭",1));

        int diff = 0;
        if (PVEManager.Instance)
        {
            diff = (int)PVEManager.Instance.difficulty;
        }
        else
        {
            diff = (int)(FightingManager.Instance.roundManager.elapsedTime / 60);
        }

       
        AddMaxHp(150*diff);
        liveWeapon.attackValue = diff;
        lastHp = props.hp;
    }
    
    void UpdateHpUIByNameText(int hp,int maxHp,int shield,int maxShield)
    {
        hpUI.SetNameText(hp+"/"+maxHp);
    }

    public override BattleUnitProps.HpAndShield OnAttacked(AttackInfo attackInfo)
    {
        var sub = (props.maxHp / 10);
        if (lastHp/sub > props.hp/sub)//血量每掉10%触发一次Impact
        {
            animator.SetTrigger("Impact");
            Debug.LogError("Impact"+lastHp/sub+","+props.hp/sub);
            lastHp = props.hp;
            
        }
        return base.OnAttacked(attackInfo);
        
    }


   


    public void MeleeAttack()
    {
        liveWeapon.pushBackHeight = 9;
        liveWeapon.pushBackStrength = 9;
        var enemys = AttackManager.Instance.GetEnemyInRadius(this, transform.position, 15,9);
        foreach (var victim in enemys)
        {
            var maxValue =  victim.GetVictimEntity().props.maxHp;
            var value =liveWeapon.GetBaseAttackInfo().value+ Mathf.CeilToInt(maxValue * 0.4f);
            if (victim as Planet)
            {
                Debug.Log("攻击基地，伤害减半");
                value = Mathf.CeilToInt(value * 0.5f);
            }
                
            liveWeapon.DamageOther(victim,new AttackInfo(this,AttackType.Physics,value));
        }
    }
    
    public void ImpactFx()
    {
        
        //Debug.LogError("Impact");
        liveWeapon.pushBackHeight = 9;
        liveWeapon.pushBackStrength = 9;
        var enemys = AttackManager.Instance.GetEnemyInRadius(this, transform.position, 35,9);
        foreach (var victim in enemys)
        {
            var maxValue =  victim.GetVictimEntity().props.maxHp;
            var value = liveWeapon.GetBaseAttackInfo().value+Mathf.CeilToInt(maxValue * 0.6f);
            if (victim as Planet)
            {
                Debug.Log("攻击基地，伤害减半");
                value = Mathf.CeilToInt(value * 0.5f);
            }

            
            liveWeapon.DamageOther(victim,new AttackInfo(this,AttackType.Real,value));

        }
    }
    private void OnDisable()
    {
        onHpChanged -= UpdateHpUIByNameText;
    }

    public override void Die()
    {
        base.Die();
        PVEManager.Instance.GameWin();
    }
}
