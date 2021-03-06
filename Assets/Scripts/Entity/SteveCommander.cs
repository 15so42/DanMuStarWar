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
    public List<McUnit> battleUnits=new List<McUnit>();


    public SteveWeaponNbt steveWeaponNbt;
    //武器耐久
    // public bool weaponSaved;
    public int desireWeaponId;
    // public int endurance;//武器耐久值
    // public int vampire = 0;
    // public int fire = 0;
    // public int parry = 0;
    // public int triumph = 0;

    public int desireSpellCount=3;


    public int leftSpecificSpell = 2;

    private bool surrendered = false;

    //每局任意礼物可获得一次额外次数
    public bool flowerSpell = false;
    
    //复活timer
    public UnityTimer.Timer unityTimer;
    
    //兑换礼物武器次数
    public int giftWeaponCount = 0;
    
    public SteveCommander(int uid, Player player) : base(uid, player)
    {
    }

    public SteveCommander(int uid, Player player, Color color) : base(uid, player, color)
    {
    }

    public void SetMaxSpellCount()
    {
        var steve = FindFirstValidSteve();
        steve.GetActiveWeapon().SetMaxSpellCount(desireSpellCount);
    }

    public override void Init(Planet ownerPlanet)
    {
        base.Init(ownerPlanet);
        EventCenter.AddListener<Steve>(EnumEventType.OnSteveDied,OnSteveDie);
        point = 0;
        AddPoint(0);
        desireSpellCount = 3;

        if (player.userSaveData != null)
        {
            OnSetUserData();
        }
        else
        {
            player.onSetUserData += OnSetUserData;
        }

        

        //绑定事件检测自己的单位得产生
        //EventCenter.AddListener(EnumEventType.OnBattleUnitCreated,OnSteveCreated);
    }

    public void ParseSurrenderInMc()
    {
        surrendered = true;
        UnityTimer.Timer.Register(120, () =>
        {
            surrendered = false;
            MessageBox._instance.AddMessage("系统", player.userName + "取消了投降");

        });

        var surrenderCount = 0;
        var commandersCount = ownerPlanet.planetCommanders.Count;
        for (int i = 0; i < commandersCount; i++)
        {
            if ((ownerPlanet.planetCommanders[i] as SteveCommander).surrendered)
            {
                surrenderCount++;
            }
        }
        TipsDialog.ShowDialog(player.userName+"发起了投降,["+surrenderCount+"/"+commandersCount+"]",null);
        MessageBox._instance.AddMessage("系统", player.userName + "发起了投降,[" + surrenderCount + "/" + commandersCount + "]");
        if ((float) surrenderCount / commandersCount > 0.5f)
        {
            TipsDialog.ShowDialog("投降成功",null);
            ownerPlanet.Die();
        }
        
    }

    void OnSetUserData()
    {
        var userSaveData = player.userSaveData;
        //desireMaxHp = 20 + Mathf.CeilToInt((float)userSaveData.giftPoint / 40);
        desireMaxHp = 25;
        leftSpecificSpell += Mathf.CeilToInt((float)userSaveData.giftPoint / 300);
        
        
        if (player.uid==402554900)//绝言功能，额外附魔槽位，额外初始点数
        {
            desireSpellCount++;
            AddPoint(30);
        }

        #if UNITY_EDITOR
        if (player.uid == 1 || player.uid == 23204263)//DEbug专用额外点数
        {
            AddPoint(150);
        }
        #endif

        // if (player.uid == 1460630713  )//阿斯达10滴血
        // {
        //     desireMaxHp = 10;
        // }

        // if (player.uid == 1834685283)//小梦荒野5滴血
        // {
        //     desireMaxHp = 10;
        // }
        
        
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
            if (battleUnits[i] && battleUnits[i].die == false && battleUnits[i].GetType()==typeof(Steve))
                return battleUnits[i] as Steve;
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
            if (time > 90)
                time = 90;
            if(commanderUi!=null)
                (commanderUi as SteveCommanderUi)?.StartCountDown((int)time);
            unityTimer?.Cancel();
            unityTimer=Timer.Register(time, () => { CreateSteve(); });
        }
        
    }

    public override void Update()
    {
        if (player.uid == 8045498 )
        {
            AddPoint(0.3f);
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
        FightingManager.Instance.exitPlayers.Add(player.uid);
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
