using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 局内处理
/// </summary>
public class RoundManager : MonoBehaviour
{
    private GameManager gameManager;
    private List<Player> players=new List<Player>();
    public void Init(GameManager gameManager, List<Player> players)
    {
        this.gameManager = gameManager;
        this.players = players;
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
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
        return PlanetManager.Instance.allPlanets[index];
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
    
    void ParseChangeSkill(int uid,string trim)
    {
        string pattern = @"^(换技能){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析换技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            planet.ChangeSkill(skillIndex);
            
           
        }
    }
    
    void ParseUseSkill(int uid,string trim)
    {
        string pattern = @"^(使用技能){1}(\d{1})$";
        if (Regex.IsMatch(trim, pattern))
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
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析移除技能命令:"+trim);
            int skillIndex = Int32.Parse(trim.Substring(trim.Length-1,1));

            var planet=GetPlantByPlayerUid(uid);
            planet.RemoveSkill(skillIndex);
            
           
        }
    }
    
    void ParseGetSkill(int uid,string trim)
    {
        string pattern = @"^(抽取技能)$";
        if (Regex.IsMatch(trim, pattern))
        {
            Debug.Log("解析抽取技能命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            planet.GetSkill();
            
           
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
        
        if (text.StartsWith("使用技能"))
        {
            ParseUseSkill(uid, trim);
        }
        
        if (text.StartsWith("换技能"))
        {
            ParseChangeSkill(uid, trim);
        }
        
        if (text.StartsWith("移除技能"))
        {
            ParseRemoveSkill(uid, trim);
        }
        
        if (text.StartsWith("抽取技能"))
        {
            ParseGetSkill(uid, trim);
        }
        
        if (text.StartsWith("技能说明"))
        {
            ParseShowSkillDesc(uid, trim);
        }
        
        
    }

    
}
