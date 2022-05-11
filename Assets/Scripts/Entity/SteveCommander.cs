using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class SteveCommander : PlanetCommander
{
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
    }

    public void CreateSteve()
    {
        ownerPlanet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,GameConst.BattleUnit_STEVE,1),this));
    }

    public void OnSteveDie(Steve steve)
    {
        if (steve.planetCommander == this)
        {
            (commanderUi as SteveCommanderUi)?.StartCountDown(24);
            Timer.Register(24, () => { CreateSteve(); });
        }
        
    }
}
