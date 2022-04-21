using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCommander 
{

    public int uid;
    public Player player;
    public int point;
    public CommanderUI commanderUi;
    public Color color;

    public PlanetCommander (int uid, Player player)
    {
        this.uid = uid;
        this.player = player;
        point = 4;
    }
    
    public PlanetCommander (int uid, Player player,Color color)
    {
        this.uid = uid;
        this.player = player;
        point = 4;
        this.color = color;
    }

    public Action<int> onPointChanged;

    public void AddPoint(int value)
    {
        point+=value;
        onPointChanged?.Invoke(point);
    }
}
