using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class IronGolem : McUnit
{
    public Action<float> updateRebootPointAction;

    public float rebootPoint;
    public int maxRebootPoint = 2;

    private bool rebooted = false;
    
    protected override void Start()
    {
        base.Start();
        var liveWeapon = GetComponentInChildren<HandWeapon>();
        liveWeapon.Init(this);
        EventCenter.AddListener<Steve>(EnumEventType.OnSteveDied,OnSteveDie);
        EventCenter.AddListener<Planet,int>(EnumEventType.OnMcBatteryReceived,OnMcBatteryReceived);
        canPushBack = false;
        //hpUI.OpenHPTile();
        //hpUI.OpenHpNumText();

        onHpChanged += UpdateHpUIByNameText;
        UpdateHpUIByNameText(props.hp,props.maxHp,props.shield,props.maxShield);
        
    }


    void UpdateHpUIByNameText(int hp,int maxHp,int shield,int maxShield)
    {
        hpUI.SetNameText(hp+"/"+maxHp);
    }

    #region Statemachine

    public void ShutDown()
    {
        animator.SetTrigger("ShutDown");
        moveManager.SetFinalTarget(transform.position,true);
        maxRebootPoint = fightingManager.players.Count * 7;
    }

    #endregion


    public void AddRebootPoint(float point)
    {
        rebootPoint += point;
        updateRebootPointAction?.Invoke(point);
        //var rate = Mathf.CeilToInt(((float)rebootPoint / point)*10);
        var str = "重启进度：|";

        str += (float)(rebootPoint*100) / maxRebootPoint +"%";

        str += "|";
        LogTip(str);

        if (rebootPoint >= maxRebootPoint)
        {
            Reboot();
        }
        
    }

    string GetProgress()
    {
        return (float)(rebootPoint*100) / maxRebootPoint +"%";
    }
    
    void Reboot()
    {
        if(rebooted)
            return;
        Debug.Log("Reboot");
        CustomEvent.Trigger(gameObject, "Reboot");
        //血量计算
        animator.SetTrigger("ReBoot");

        
        int addHp=fightingManager.players.Count * (Mathf.CeilToInt(fightingManager.roundManager.elapsedTime/300)* 15);
        props.maxHp += addHp;
        
        OnAttacked(new AttackInfo(this,AttackType.Heal,addHp));
        rebooted = true;
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<Steve>(EnumEventType.OnSteveDied,OnSteveDie);
        EventCenter.RemoveListener<Planet,int>(EnumEventType.OnMcBatteryReceived,OnMcBatteryReceived);
        onHpChanged -= UpdateHpUIByNameText;
    }

    public void OnSteveDie(Steve steve)
    {
        if(rebooted)
            return;
        if (steve.ownerPlanet == ownerPlanet)
        {
            AddRebootPoint(2);
        }
        else
        {
            AddRebootPoint(1);
        }
    }

    void OnMcBatteryReceived(Planet planet, int battery)
    {
        if (ownerPlanet.enemyPlanets.Contains(planet) && !rebooted)
        {
            props.maxHp += battery*2;
            OnAttacked(new AttackInfo(this,AttackType.Heal,battery*2));
            AddRebootPoint((float)battery/2);
            LogTip("HP+"+battery*2+" 重启进度:"+GetProgress());
        }
    }

    public override BattleUnitProps.HpAndShield OnAttacked(AttackInfo attackInfo)
    {
        if (props.hp <= props.maxHp * 0.5f && !rebooted && attackInfo.attackType!=AttackType.Heal)
        {
            Reboot();
        }
        return base.OnAttacked(attackInfo);
    }

    public override void Die()
    {
        base.Die();
        //ownerPlanet.RefreshIronGolem();
    }
}
