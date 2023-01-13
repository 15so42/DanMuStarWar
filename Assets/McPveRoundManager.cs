using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McPveRoundManager : McRoundManager
{
    private GameObject fireWall;
    private UnityTimer.Timer fireWallTimer;
    protected override void ParseGiftInMcMode(SteveCommander steveCommander, string giftName, int battery)
    {
        base.ParseGiftInMcMode(steveCommander, giftName, battery);
        
        if (giftName == "打call")
        {
            ParseRandomSpell(steveCommander,false,false);//byGift本来是会开辟新的附魔槽位的，现在不会再开辟了
        }
        
        if (giftName == "牛哇牛哇" || giftName == "牛哇")
        {
            ParseAddPoint(steveCommander);
        }
        
        if (giftName == "flag")
        {
            //ParseAddMaxHp(steveCommander,true);
        }
        
        if (giftName == "情书")
        {
            return;
            fireWall=GameObject.FindGameObjectWithTag("FireWall");

            for (int i = 0; i < fireWall.transform.childCount; i++)
            {
                fireWall.transform.GetChild(i).gameObject.SetActive(true);
            }
            fireWall.gameObject.SetActive(true);
            fireWallTimer = UnityTimer.Timer.Register(180, () =>
            {
                for (int i = 0; i < fireWall.transform.childCount; i++)
                {
                    fireWall.transform.GetChild(i).gameObject.SetActive(false);
                }
            });
        }

        if (giftName == "小花花")
        {
            //steveCommander.SummonGolem();
            var player = steveCommander.player;
            if (player.userSaveData != null)
            {
                player.userSaveData.coin += 40;
                MessageBox._instance.AddMessage("系统",player+"获得20绿宝石");
            }
        }

        if (giftName == "快乐水")
        {
            SelfExplosionSheep(steveCommander);
            
        }
    }

    public void SelfExplosionSheep(SteveCommander steveCommander)
    {
        for (int i = 0; i < 10; i++)
        {
            steveCommander.ownerPlanet.AddTask(new PlanetTask(new TaskParams(TaskType.Create, "BattleUnit_ExplosionSheep",0.1f),steveCommander ));

        }
    }

    private void OnDisable()
    {
        fireWallTimer?.Cancel();
    }
}
