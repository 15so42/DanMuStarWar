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

    public void ImpactFx()
    {
        
        //Debug.LogError("Impact");

        var enemys = AttackManager.Instance.GetEnemyInRadius(this, transform.position, 25,9);
        foreach (var victim in enemys)
        {
            liveWeapon.DamageOther(victim,new AttackInfo(this,AttackType.Real,Mathf.CeilToInt(victim.GetVictimEntity().props.hp*0.5f)));

            var victimEntity = victim.GetVictimEntity();
            var navMove = victimEntity.GetComponent<NavMeshMoveManager>();
            if (navMove)
            {
                navMove.PushBackByPos( victimEntity.transform.position,transform.position,12,9,1);
            }

        }
    }
    private void OnDisable()
    {
        onHpChanged -= UpdateHpUIByNameText;
    }
}
