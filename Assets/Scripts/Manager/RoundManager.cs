using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// 局内处理
/// </summary>
public class RoundManager
{
    
    private float timer = 0;
    
    private GameManager gameManager;
    private List<Player> players=new List<Player>();
    public void Init(GameManager gameManager, List<Player> players)
    {
        this.gameManager = gameManager;
        this.players = players;
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >180)
        {
            GameEnvEventManager.Instance.PlayRandomEvent();
            timer = 0;
        }
    }

    /// <summary>
    /// 停止游戏内回合或者说是时间处理
    /// </summary>
    public void Stop()
    {
        EventCenter.RemoveListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
    }

    private void OnDanMuReceived(string userName,int uid,string time,string text)
    {
        ParseCommand(uid,text);
            
    }

    Player GetPlayerByUid(int uid)
    {
        return players.Find(x => x.uid == uid);
    }

    Planet GetPlantByPlayerUid(int uid)
    {
        return PlanetManager.Instance.allPlanets.Find(x => x.owner.uid == uid);
    }

    Planet GetPlanetByIndex(int index)
    {
        return PlanetManager.Instance.allPlanets.Find(x => x.planetIndex == index);
    }

    void ParseClaimWar(int uid,string trim)
    {
        string pattern = @"^(宣战){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析宣战命令:"+trim);
            int enemyIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var attckerPlanet = GetPlantByPlayerUid(uid);
            var victimPlanet = GetPlanetByIndex(enemyIndex);

            attckerPlanet.ClaimWar(victimPlanet);
            victimPlanet.ClaimWar(attckerPlanet);
        }
    }

    void ParseDefend(int uid, string trim)
    {
        string pattern = @"^(驻守){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析驻守命令:"+trim);
            int targetIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var attckerPlanet = GetPlantByPlayerUid(uid);
            var target = GetPlanetByIndex(targetIndex);

            
            attckerPlanet.ClaimDefend(target);
            
        }
    }
    
    void ParseChangeSkill(int uid,string trim)
    {
        string pattern = @"^(换技能){1}(\d{1})$";
        string letterPattern = @"^(h|H){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) || Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析换技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            planet.ChangeSkill(skillIndex);
            
           
        }
    }
    
    void ParseUseSkill(int uid,string trim)//使用技能1或者u1或者U1
    {
        string pattern = @"^(使用技能){1}(\d{1})$";
        string letterPattern = @"^(s|S){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) ||Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析使用技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            planet.UseSkill(skillIndex);
            
           
        }
    }
    
    void ParseRemoveSkill(int uid,string trim)
    {
        string pattern = @"^(移除技能){1}(\d{1})$";
        string letterPattern = @"^(y|Y){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern) || Regex.IsMatch(trim, letterPattern))
        {
            Debug.Log("解析移除技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            planet.RemoveSkill(skillIndex);
            
           
        }
    }
    
    void ParseRollSkill(int uid,string trim)
    {
        string pattern = @"^(抽取技能)$";
        if (Regex.IsMatch(trim, pattern) || trim=="c" || trim=="C")
        {
            Debug.Log("解析抽取技能命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            planet.RollSkill();
            
           
        }
    }
    
    void ParseShowSkillDesc(int uid,string trim)
    {
        string pattern = @"^(技能说明)$";
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析查看技能说明命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            planet.skillContainer.ShowSkillDesc();


        }
    }
    
    //解析命令
    private void ParseCommand(int uid, string text)
    {
        var validUser = (GetPlayerByUid(uid) != null);
        if (!validUser)
        {
            //局外人
            return;
        }
        var trim=Regex.Replace(text.Trim(), "\\s+", "");//去除所有空格
        
        if (text.StartsWith("宣战"))
        {
            ParseClaimWar(uid, trim);
        }
        
        if (text.StartsWith("驻守"))
        {
            ParseDefend(uid, trim);
        }
        
        if (text.StartsWith("使用技能")||text.StartsWith("s")||text.StartsWith("S"))
        {
            ParseUseSkill(uid, trim);
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
        
        if (text.StartsWith("技能说明"))
        {
            ParseShowSkillDesc(uid, trim);
        }
        
        
    }

    
}
