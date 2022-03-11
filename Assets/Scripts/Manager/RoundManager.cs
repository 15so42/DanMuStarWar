using System.Collections;
using System.Collections.Generic;
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
    
    //解析命令
    private void ParseCommand(int uid, string text)
    {
        var validUser = (GetPlayerByUid(uid) != null);
        if (!validUser)
        {
            //局外人
            return;
        }
        
        
    }
}
