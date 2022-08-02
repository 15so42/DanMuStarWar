using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : McUnit
{

    private DayLightManager dayLightManager;
    
    protected override void Start()
    {
        base.Start();
        var liveWeapon = GetComponentInChildren<HandWeapon>();
        liveWeapon.Init(this);
       
        hpUI.OpenHPTile();
        
        var elapsedTime = FightingManager.Instance.roundManager.elapsedTime;
        props.maxHp += (int)(elapsedTime * 0.01);
        OnAttacked(new AttackInfo(this, AttackType.Heal, (int)(elapsedTime * 0.01)));
       
        dayLightManager=DayLightManager.Instance;

        liveWeapon.attackValue += (int) (elapsedTime / 450);
        LogTip("血量+"+(int)(elapsedTime * 0.01)+",攻击力+"+(int) (elapsedTime / 450));
    }

    private void Update()
    {
        if (dayLightManager.IsDay())
        {
            SkillManager.Instance.AddSkill("Skill_着火_LV1", this, null);
        }
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject,10);
    }


    public override IVictimAble EnemyCheck(Collider collider)
    {
        
        if (collider.GetComponent<Planet>() && canSetPlanetEnemy==false)
            return null;
        return base.EnemyCheck(collider);
    }
}
