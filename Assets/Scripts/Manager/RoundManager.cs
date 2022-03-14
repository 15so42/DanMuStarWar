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
        
        
    }
}
