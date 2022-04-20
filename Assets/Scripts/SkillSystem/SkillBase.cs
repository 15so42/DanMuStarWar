using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : ScriptableObject
{

    
    [HideInInspector]public PlanetCommander planetCommander;
    
    [Header("点数")] public int usePoint=1;
    public int removePoint = 1;
    [Header("描述")]
    public string desc="无描述";
    //注入UI，用以排序，虽然有点打乱独立性，但是影响很小
    [Header("注入UI用以排序")]
    [HideInInspector]public SkillItemUI skillItemUi;//不用这个ui要实现排序太过复杂，只能这样了
    
    public string skillName;
    public Sprite icon;
    [Header("是否是Buff[暂未使用]")] public bool isBuff;
    
    public int cd;//冷却时间
    [HideInInspector]public float timer = 0;

    [Header("可使用次数,-1表示无限次")]
    public int life=-1;

    public bool autoLife=true;

    [Header("可堆叠层数")] public int stackAbleCount = 1;
    [Header("被动技能，就绪后无需等待命令直接Act")]
    public bool passive;

    [HideInInspector]public GameEntity gameEntity;
    [HideInInspector]public bool ready = true;//主动节能是否准备完成
    [HideInInspector]public bool finished = false;//技能是否完成,SkillContainer自动清除已经完成的技能和buff
    
    //事件
    public Action onFinished;
    public Action<int> onLifeChangedAction;
    public virtual void Init(GameEntity gameEntity,PlanetCommander planetCommander)
    {
        timer = cd;
        this.gameEntity = gameEntity;
        this.planetCommander = planetCommander;
        onLifeChangedAction += OnLifeChanged;
    }

    public void ResetTimer()
    {
        timer = cd;
    }

    //上层调用，主动释放
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

    public void ChangeLife(int newLife)
    {
        life = newLife;
        onLifeChangedAction.Invoke(newLife);
    }
    public virtual ErrorCode PlayCheck()
    {
        if(finished)
            return null;
        
        if(!ready)
            return new ErrorCode(ErrorType.Failure,"技能尚未准备好");
        Play();
        if (autoLife)
        {
            ChangeLife(--life);
        }

        
        
        return new ErrorCode(ErrorType.Success,"成功");
    }

    public void OnLifeChanged(int life)
    {
        if (life == 0)
        {
            onFinished?.Invoke();
            finished = true;
        }
        else
        {
            timer = cd;//重置冷却时间
            
        }
    }

    public void SetItemIndex(int index)
    {
        skillItemUi.skillIndex.text = index+1 + "";
    }
    

    public float GetLeftCdRatio()
    {
        return (float) timer / cd;
    }

    protected virtual void Play()
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

    public virtual void Kill()
    {
        skillItemUi.Kill();
        Destroy(this);
    }

}
