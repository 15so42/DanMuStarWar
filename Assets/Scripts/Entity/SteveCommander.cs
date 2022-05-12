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
    }

    public void CreateSteve()
    {
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
        if (steve.planetCommander == this)
        {
            die = true;
            var time = FightingManager.Instance.roundManager.elapsedTime/10;
            (commanderUi as SteveCommanderUi)?.StartCountDown((int)time);
            unityTimer?.Cancel();
            unityTimer=Timer.Register(time, () => { CreateSteve(); });
        }
        
    }
}
