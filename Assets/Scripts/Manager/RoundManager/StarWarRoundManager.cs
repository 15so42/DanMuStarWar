using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine;

public class StarWarRoundManager : RoundManager
{
    protected override void ParseTrim(long uid, string text,string trim)
    {
        base.ParseTrim(uid, text,trim);
        if (text.StartsWith("宣战"))
        {
            ParseClaimWar(uid, trim);
        }
        
        if (text.StartsWith("驻守"))
        {
            ParseDefend(uid, trim);
        }
        
        if (text == "召回全部")
        {
            ParseRecallAll(uid);
        }
        
        if (text.StartsWith("召回") )
        {
            ParseRecall(uid, trim);
        }
        
        if (text.StartsWith("集结") )
        {
            ParseGather(uid, trim);
        }
            
        if (text.StartsWith("使用技能")||text.StartsWith("s")||text.StartsWith("S"))
        {
            ParseUseSkill(uid, trim);
        }
        
        if (text.StartsWith("买技能")||text.StartsWith("m")||text.StartsWith("M"))
        {
            ParseBuySkill(uid, trim);
        }
        
        if (text.StartsWith("换技能") || text.StartsWith("h") || text.StartsWith("H"))
        {
            ParseChangeSkill(uid, trim);
        }
        
        if (text.StartsWith("移除技能") || text.StartsWith("y") || text.StartsWith("Y"))
        {
            ParseRemoveSkill(uid, trim);
        }
        
        if (text.StartsWith("抽取技能") || text=="c" || text=="C")
        {
            ParseRollSkill(uid, trim);
        }
        if (text == "关闭自动抽卡")
        {
            ParseCloseAutoRoll(uid,trim);
        }
        if (text == "开启自动抽卡")
        {
            ParseOpenAutoRoll(uid,trim);
        }

        if (text == "紧急维修")
        {
            ParseUrgentRepair(uid,trim);
        }
            
        if (text.StartsWith("技能说明"))
        {
            ParseShowSkillDesc(uid, trim);
        }
        
        if (text.Equals("我在哪"))
        {
            ParseShowWhere(uid);
        }

        if (text == "偷袭")
        {
            ParseSurrender(uid);
        }
    }


    void ParseClaimWar(long uid,string trim)
    {
        string pattern = @"^(宣战){1}(\d{1,2})$";
        if (Regex.IsMatch(trim, pattern))
        {
            
            var subStringCount = 1;
            if(Regex.IsMatch(trim, @"\d{2}$"))
            {
                subStringCount = 2;
            }
            
            int enemyIndex = Int32.Parse(trim.Substring(trim.Length-subStringCount,subStringCount));
            Debug.Log("解析宣战命令:"+enemyIndex);
            var attckerPlanet = GetPlantByPlayerUid(uid);
            if(attckerPlanet==null)
                return;
            var victimPlanet = GetPlanetByIndex(enemyIndex);
            if(victimPlanet==null)
                return;

            attckerPlanet.ClaimWar(uid,victimPlanet);//
            //victimPlanet.ClaimWar(attckerPlanet);
        }
    }
    
    void ParseRecall(long uid,string trim)
    {
        string pattern = @"^(召回){1}(\d{1,2})$";
        string letterPattern = @"^(z|Z){1}(\d{1,2})$";
        if (Regex.IsMatch(trim, pattern) || Regex.IsMatch(trim, letterPattern))
        {
           
            var subStringCount = 1;
            if(Regex.IsMatch(trim, @"\d{2}$"))
            {
                subStringCount = 2;
            }
            int enemyIndex = Int32.Parse(trim.Substring(trim.Length-subStringCount,subStringCount));
            Debug.Log("解析召回命令:"+enemyIndex);
            var uidPlanet = GetPlantByPlayerUid(uid);
            if(uidPlanet==null)
                return;
            var defendPlanet = GetPlanetByIndex(enemyIndex);

            uidPlanet.Recall(uid,defendPlanet);
            
        }
    }

    void ParseRecallAll(long uid)
    {
        var uidPlanet = GetPlantByPlayerUid(uid);
        if(uidPlanet==null)
            return;
        uidPlanet.RecallAll(uid);
    }
    
    void ParseGather(long uid,string trim)
    {
        string pattern = @"^(集结){1}(\d{1,2})$";
        
        if (Regex.IsMatch(trim, pattern))
        {
           
            var subStringCount = 1;
            if(Regex.IsMatch(trim, @"\d{2}$"))
            {
                subStringCount = 2;
            }
            int targetIndex = Int32.Parse(trim.Substring(trim.Length-subStringCount,subStringCount));
            Debug.Log("解析集结命令:"+targetIndex);
            var uidPlanet = GetPlantByPlayerUid(uid);
            if(uidPlanet==null)
                return;
            
            
            var targetPlanet = GetPlanetByIndex(targetIndex);

            if(targetPlanet)
                uidPlanet.Gather(uid,targetPlanet);
            
        }
    }

    void ParseDefend(long uid, string trim)
    {
        string pattern = @"^(驻守){1}(\d{1,2})$";
        if (Regex.IsMatch(trim, pattern))
        {
            
            var subStringCount = 1;
            if(Regex.IsMatch(trim, @"\d{2}$"))
            {
                subStringCount = 2;
            }
           
            int targetIndex = Int32.Parse(trim.Substring(trim.Length-subStringCount,subStringCount));
            Debug.Log("解析驻守命令:"+targetIndex);
            var attckerPlanet = GetPlantByPlayerUid(uid);
            if(attckerPlanet==null)
                return;
            var target = GetPlanetByIndex(targetIndex);

            
            attckerPlanet.ClaimDefend(uid,target);
            
        }
    }

    void ParseComplexCommand(long uid, string trim)
    {
        string pattern= @"((m|M){1}\d{1}|(s|S){1}\d{1}|(y|Y){1}\d{1}|(h|H){1}\d{1})+";
        
    }
    
    void ParseChangeSkill(long uid,string trim)
    {
        string pattern = @"^(换技能){1}(\d{1})$";
        string letterPattern = @"^(h|H){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) || Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析换技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

          
            
            var planet=GetPlantByPlayerUid(uid);
            if (planet)
            {
                
                    planet.ChangeSkill(uid,skillIndex);
                
            }
           
        }
    }
    
    void ParseUseSkill(long uid,string trim)//使用技能1或者u1或者U1
    {
        string pattern = @"^(使用技能){1}(\d{1})$";
        string letterPattern = @"^(s|S){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) ||Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析使用技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.UseSkill(uid,skillIndex);
            
           
        }
    }
    
    void ParseBuySkill(long uid,string trim)//使用技能1或者u1或者U1
    {
        string pattern = @"^(买技能){1}(\d{1})$";
        string letterPattern = @"^(m|M){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) ||Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析买技能命令:"+trim);
            int index = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            if (planet)
            {
                //if (fightingManager.gameMode == GameMode.BattleGround)
                    planet.BuySkillBG(uid, index);
                // else 
                //     planet.BuySkill(uid,index);
            }
         
        }
    }
    
    void ParseRemoveSkill(long uid,string trim)
    {
        string pattern = @"^(移除技能){1}(\d{1})$";
        string letterPattern = @"^(y|Y){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) || Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析移除技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.RemoveSkill(uid,skillIndex);
            
           
        }
    }
    
    void ParseRollSkill(long uid,string trim)
    {
        string pattern = @"^(抽取技能)$";
        if (Regex.IsMatch(trim, pattern) || trim=="c" || trim=="C")
        {
            Debug.Log("解析抽取技能命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if (planet)
            {
                
                planet.RollSkill(uid);
                
            }
                
            
           
        }
    }
    
    void ParseShowSkillDesc(long uid,string trim)
    {
        string pattern = @"^(技能说明)$";
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析查看技能说明命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.skillContainer.ShowSkillDesc();


        }
    }
    
    void ParseUrgentRepair(long uid,string trim)
    {
        if (trim == "紧急维修")
        {
            Debug.Log("解析紧急维修命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.UrgentRepair(uid);
        }
    }
    
    void ParseCloseAutoRoll(long uid,string trim)
    {
        if (trim == "关闭自动抽卡")
        {
            Debug.Log("解析关闭自动抽卡命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.autoRoll=false;
        }
    }
    
    void ParseOpenAutoRoll(long uid,string trim)
    {
        if (trim == "开启自动抽卡")
        {
            Debug.Log("解析开启自动抽卡命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.autoRoll=true;
        }
    }
    
    void ParseSurrender(long uid)
    {
        
            Debug.Log("解析投降命令");
           
            var planet=GetPlantByPlayerUid(uid);
            if (planet)
                planet.Die();
        
    }

    void ParseShowWhere(long uid)
    {
        var planet=GetPlantByPlayerUid(uid);
        if (planet)
            planet.ShowCommanderPosition(uid); 
    }

    
    /******************解析礼物*********************/
    
    protected override void ParseGift(long uid, string giftName,long battery)
    {
        if (giftName == "小花花")
        {
        }
        if (giftName == "打call")
        {
            var planet = GetPlantByPlayerUid(uid);
            if (planet)
            {
                var unitName = GameConst.BattleUnit_COLLECTOR;
                var range = UnityEngine.Random.Range(0, 100);
                if (range < 50 && range > 25)
                    unitName = GameConst.BattleUnit_WARPLANE;
                if (range < 75 && range > 50)
                    unitName = GameConst.BattleUnit_GUARDPLANE;
                if (range < 90 && range > 75)
                    unitName = GameConst.BattleUnit_LONGBOW;
                if (range < 100 && range > 90)
                    unitName = GameConst.BattleUnit_PACMAN;
                

                var planetCommander = planet.GetCommanderByUid(uid);
                if (planetCommander != null)
                {
                    planet.AddTask(new PlanetTask(new TaskParams(TaskType.Create,unitName,1),planetCommander));
                    planetCommander.commanderUi.LogTip("礼物造兵:"+unitName.Split('_')[1]);
                }
                
            }
           
        }
    }
}
