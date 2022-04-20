using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCommander 
{

    public int uid;
    public Player player;

    public PlanetCommander (int uid, Player player)
    {
        this.uid = uid;
        this.player = player;
    }

    public Action<int> onPointChanged;
}
