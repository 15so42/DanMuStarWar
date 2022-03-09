﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    
    public int cd;//冷却时间
    [HideInInspector]private float timer = 0;

    [Header("可使用次数,-1表示无限次")]
    public int life=-1;

    public bool passive;

    [HideInInspector]public GameEntity gameEntity;
    [HideInInspector]public bool ready = true;//主动节能是否准备完成
    [HideInInspector]public bool finished = false;//技能是否完成
    
    //事件
    public Action onFinished;
    public Action<int> onLifeChanged;
    public virtual void Init(GameEntity gameEntity)
    {
        timer = cd;
        this.gameEntity = gameEntity;
    }

    //上层调用
    public virtual bool Use()
    {
        var errCode = PlayCheck();
        if (errCode == null || errCode.code == ErrorType.Success)
        {
            return true;
        }
        
        gameEntity.LogTip(errCode.errorStr);
        return false;
    }

    private ErrorCode PlayCheck()
    {
        if(finished)
            return null;
        
        if(!ready)
            return new ErrorCode(ErrorType.Failure,"技能尚未准备好");
        Play();

        life--;
        onLifeChanged?.Invoke(life);
        if (life == 0)
        {
            finished = true;
            onFinished?.Invoke();
        }
        else
        {
            timer = cd;//重置冷却时间
            
        }
        return new ErrorCode(ErrorType.Success,"成功");
    }

    public float GetLeftCdRatio()
    {
        return (float) timer / cd;
    }

    public virtual void Play()
    {
        
    }

    public virtual void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ready = true;
            if (passive)//被动技能直接释放
            {
                PlayCheck();
            }
            
        }
    }

}
