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
        props.maxHp += (int)(elapsedTime * 0.05f);
        props.hp+=(int)(elapsedTime * 0.05f);
        dayLightManager=DayLightManager.Instance;
    }

    private void Update()
    {
        if (dayLightManager.IsDay())
        {
            SkillManager.Instance.AddSkill("Skill_着火_LV1", this, null);
        }
    }
}
