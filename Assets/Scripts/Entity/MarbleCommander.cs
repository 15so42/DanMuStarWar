using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class MarbleCommander : SteveCommander
{
    public MarbleCommander(int uid, Player player) : base(uid, player)
    {
    }

    public MarbleCommander(int uid, Player player, Color color) : base(uid, player, color)
    {
    }
}
