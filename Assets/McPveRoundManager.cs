using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameCode.Tools;
using UnityEngine;
using Random = System.Random;

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
                MessageBox._instance.AddMessage("系统",player.userName+"获得40绿宝石");
            }
        }

        if (giftName == "红灯笼")
        {
            try
            {
                var rand = UnityEngine.Random.Range(0, 100);
                var num = 400;
                if (rand < 50)//50
                {
                    num = 400;
                }

                if (rand >= 50 && rand < 75)//25
                {
                    num = 650;
                }

                if (rand >= 75 && rand < 90)//15
                {
                    num = 900;
                }

                if (rand >= 90)//10
                {
                    num = 1200;
                }

                var commanders = steveCommander.ownerPlanet.planetCommanders;
                num *= (int)(commanders.Count * 0.2f);
                var queue = DivideRedPacket(num, commanders.Count);
                foreach (var commander in commanders)
                {

                    var coinCount = (int)(float)queue.Dequeue();
                    
                    
                    if (commander.player.userSaveData != null)
                    {
                        commander.player.userSaveData.AddCoin(coinCount);
                        commander.commanderUi.LogTip("绿宝石"+coinCount);
                        MessageBox._instance.AddMessage("系统",commander.player.userName+"通过红包获得"+coinCount+"绿宝石");
                    }
                    
                    var steve = (commander as SteveCommander).FindFirstValidSteve();
                    if (steve != null)
                    {
                        FlyText.Instance.ShowDamageText(steve.GetVictimPosition(),"绿宝石+"+coinCount,Color.green);
                        ResFactory.Instance.CreateFx("RedPacketFx", steve.GetVictimEntity().transform.position);
                        

                    }
                }
                
            }
            catch (Exception e)
            {
                TipsDialog.ShowDialog("红灯笼Exception"+e.Message,null);
                Debug.LogError("红灯笼Exception"+e.Message);
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
    
    
    public static Queue DivideRedPacket(float money_YUAN, int peopleNum)
    {
        Queue money_YUAN_Queue = new Queue();
        int reset_FEN = (int)money_YUAN * 100;
        int resetPeopleNum = peopleNum;
        Random random = new Random();
        for (int i = 0; i < peopleNum-1; i++)
        {
            // 随机范围：[1, 剩余人均金额的2倍-1]分
            int curMoney_FEN = random.Next(1, reset_FEN/resetPeopleNum * 2 - 1);
            money_YUAN_Queue.Enqueue((float)curMoney_FEN / 100);
            reset_FEN -= curMoney_FEN;
            resetPeopleNum--;
        }
        money_YUAN_Queue.Enqueue((float)reset_FEN / 100);
        return money_YUAN_Queue;
    }

    

    private void OnDisable()
    {
        fireWallTimer?.Cancel();
    }
}
