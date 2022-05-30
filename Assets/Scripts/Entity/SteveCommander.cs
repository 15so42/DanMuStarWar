using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class SteveCommander : PlanetCommander
{
    //记录状态
    public bool die;
    public int desireMaxHp;//通过送礼叠加到的最大hp
    
    //记录自己控制的单位
    public List<Steve> battleUnits=new List<Steve>();


    public SteveWeaponNbt steveWeaponNbt;
    //武器耐久
    // public bool weaponSaved;
    public int desireWeaponId;
    // public int endurance;//武器耐久值
    // public int vampire = 0;
    // public int fire = 0;
    // public int parry = 0;
    // public int triumph = 0;
    
    
    //复活timer
    public UnityTimer.Timer unityTimer;
    
    public SteveCommander(int uid, Player player) : base(uid, player)
    {
    }

    public SteveCommander(int uid, Player player, Color color) : base(uid, player, color)
    {
    }

    public override void Init(Planet ownerPlanet)
    {
        base.Init(ownerPlanet);
        EventCenter.AddListener<Steve>(EnumEventType.OnSteveDied,OnSteveDie);
        point = 0;
        AddPoint(0);
        
        //绑定事件检测自己的单位得产生
        //EventCenter.AddListener(EnumEventType.OnBattleUnitCreated,OnSteveCreated);
    }

    public void CreateSteve()
    {
        if (hangUp)
        {
            return;
        }

       
        ownerPlanet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,GameConst.BattleUnit_STEVE,1),this));
        die = false;
    }

    public Steve FindFirstValidSteve()
    {
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] && battleUnits[i].die == false)
                return battleUnits[i];
        }

        return null;
    }

    public void RespawnImmediately()
    {
        (commanderUi as SteveCommanderUi).BreakCountDown();
        unityTimer?.Cancel();
        CreateSteve();
        
    }

    public void OnSteveDie(Steve steve)
    {
        if(hangUp)
            return;
        if (steve.planetCommander == this)
        {
            die = true;
            if(FightingManager.Instance==null || FightingManager.Instance.roundManager==null)
                return;
            var time = FightingManager.Instance.roundManager.elapsedTime/10;
            time *= 0.6f;
            if(commanderUi!=null)
                (commanderUi as SteveCommanderUi)?.StartCountDown((int)time);
            unityTimer?.Cancel();
            unityTimer=Timer.Register(time, () => { CreateSteve(); });
        }
        
    }
    
    void OnSteveCreated(BattleUnit battleUnit)
    {
        // if (battleUnit.planetCommander == this)
        // {
        //     (battleUnit as Steve).LoadWeaponParams();
        // }
    }

    public override void OnHangUp()
    {
        base.OnHangUp();
        (commanderUi as SteveCommanderUi)?.OnHangUp();
        for (int i = 0; i < battleUnits.Count; i++)
        {
            battleUnits[i].Die();
        }

        
        ownerPlanet.commanderUis.Remove(commanderUi);
        ownerPlanet.planetCommanders.Remove(this);
        FightingManager.Instance.players.Remove(player);
        GameObject.Destroy(commanderUi.gameObject);
        if (ownerPlanet.planetCommanders.Count == 0)
        {
            ownerPlanet.Die();
        }
        GC.Collect();


    }
}
