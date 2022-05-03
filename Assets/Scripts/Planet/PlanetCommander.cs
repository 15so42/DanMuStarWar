﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class PlanetCommander 
{

    public int uid;
    public Player player;
    public float point;
    public CommanderUI commanderUi;
    public Color color;

    public PlanetCommander (int uid, Player player)
    {
        this.uid = uid;
        this.player = player;
        point = 8;
    }
    
    public PlanetCommander (int uid, Player player,Color color)
    {
        this.uid = uid;
        this.player = player;
        point = 8;
        this.color = color;
    }

    public Action<float> onPointChanged;
    
    
    //挂机判定
    public float lastUpdateMsgTime;
    public bool hangUp;

    public void UpdateLastMsgTime(float time)
    {
        lastUpdateMsgTime = time;
    }

    public void AddPoint(float value)
    {
        point+=value;
        onPointChanged?.Invoke(point);
    }

    /// <summary>
    /// 挂机判定
    /// </summary>
    public void HangUpCheck()
    {
        if (Time.time > FightingManager.Instance.kickOutTime + lastUpdateMsgTime)
        {
            commanderUi.LogTip("触发挂机判定");
            //分发点数
            hangUp = true;
        }
    }
}
