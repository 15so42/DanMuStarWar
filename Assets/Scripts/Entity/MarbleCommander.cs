using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class MarbleCommander : SteveCommander
{
    public MarbleCommander(long uid, Player player) : base(uid, player)
    {
    }

    public MarbleCommander(long uid, Player player, Color color) : base(uid, player, color)
    {
    }
}
