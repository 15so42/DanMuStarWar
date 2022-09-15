using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class McRankDialog : Dialog
{
    private float timer=0;//刷新时间
    public float cd = 3;
    public List<RankItemUi> damageItems=new List<RankItemUi>();
    
    public static void ShowDialog()
    {
        DialogUtil.ShowDialog(nameof(McRankDialog));
    }

    public override void Show()
    {
        base.Show();
        
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > cd)
        {
            UpdateDamageRank();
            timer = 0;
        }
    }


    private FightingManager fightingManager;
    private RoundManager roundManager;
    List<PlanetCommander> GetCommanders()
    {
        List<PlanetCommander> result = new List<PlanetCommander>();
        try
        {
            var allPlanets = PlanetManager.Instance.allPlanets;
            for (int i = 0; i < allPlanets.Count; i++)
            {
                result = result.Concat(allPlanets[i].planetCommanders) .ToList();
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return result;
    }
    private void UpdateDamageRank()
    {   
        //所有星球的所有玩家的列表
        var commanders = GetCommanders();
        if (commanders != null && commanders.Count > 0)
        {
            var sum = 0;
            for (int i = 0; i < commanders.Count; i++)//计算总伤害并排序
            {
                if (commanders[i] != null)
                {
                    sum += (commanders[i] as SteveCommander).attackOtherDamage;
                }
                
            }
            //排序
            commanders.Sort((x,y)=>x.attackOtherDamage>y.attackOtherDamage?-1:1);

            sum = sum == 0 ? 1 : sum;
            var count = commanders.Count > 4 ? 4 : commanders.Count;
            
            //第一名满条，其余根据第一名的百分比来显示
            float topDamage = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i >= count)
                {
                    damageItems[i].gameObject.SetActive(false);
                    continue;
                }
               
                damageItems[i].gameObject.SetActive(true);
                
                var steveCommander = commanders[i] as SteveCommander;
                if(steveCommander==null)
                    continue;

                if (i == 0)
                {
                    topDamage = steveCommander.attackOtherDamage;
                }

                float damageRate = (float)steveCommander.attackOtherDamage / sum;
                damageItems[i].UpdateUi(i==0?1:(float)steveCommander.attackOtherDamage/topDamage,steveCommander.player.userName+"("+steveCommander.attackOtherDamage+")",(int)(damageRate*100f));
            }
        }
        else
        {
            Close();//关闭当前面板
        }
    }
}
