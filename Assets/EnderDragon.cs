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
        canPushBack = false;
        
        onHpChanged += UpdateHpUIByNameText;
        UpdateHpUIByNameText(props.hp,props.maxHp,props.shield,props.maxShield);
        lastHp = props.maxHp;

        //liveWeapon.TrySpecificSpell("龙鳞");
        liveWeapon.weaponNbt.enhancementLevels.Add(new EnhancementLevel("龙鳞",1));
        var diff = PVEManager.Instance.difficulty;
        AddMaxHp(50*(int)diff);
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
            liveWeapon.DamageOther(victim,new AttackInfo(this,AttackType.Real,Mathf.CeilToInt(victim.GetVictimEntity().props.maxHp*0.25f)));
        }
    }
    
    public void ImpactFx()
    {
        
        //Debug.LogError("Impact");
        liveWeapon.pushBackHeight = 9;
        liveWeapon.pushBackStrength = 9;
        var enemys = AttackManager.Instance.GetEnemyInRadius(this, transform.position, 25,9);
        foreach (var victim in enemys)
        {
            
            liveWeapon.DamageOther(victim,new AttackInfo(this,AttackType.Physics,Mathf.CeilToInt(victim.GetVictimEntity().props.hp*0.25f)));

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
