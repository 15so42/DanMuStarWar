using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : McUnit
{

    private DayLightManager dayLightManager;
    public bool selfFire = true;

    /// <summary>
    /// 删除不要的附魔，如凋零不能附魔烈阳
    /// </summary>
    protected virtual void RemoveSpell(HandWeapon handWeapon)
    {
        
    }
    protected override void Start()
    {
        base.Start();
        var liveWeapon = GetComponentInChildren<HandWeapon>();
        liveWeapon.Init(this);
       
        hpUI.OpenHPTile();
        
        
        var diff = PVEManager.Instance.difficulty;
        var addHp = (int) diff;
        AddMaxHp((int)diff);
        
        var addAttack= (int) (diff / 3);
        liveWeapon.attackValue += addAttack;
        LogTip("血量+"+(int)addHp+",攻击力+"+(int) addAttack);
        
        
        dayLightManager=DayLightManager.Instance;
        RemoveSpell(liveWeapon);
    }

    protected override void Update()
    {
        base.Update();
        if (selfFire && dayLightManager.IsDay())
        {
            SkillManager.Instance.AddSkill("Skill_着火_LV1", this, null);
        }
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject,600);
    }


    public override IVictimAble EnemyCheck(Collider collider)
    {
        
        if (collider.GetComponent<Planet>() && canSetPlanetEnemy==false)
            return null;
        return base.EnemyCheck(collider);
    }
}
