using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum GameStatus
{
    Init,//程序初始化状态
    WaitingJoin,//等待玩家加入
    CountDownToFight,//玩家加入完毕后倒计时
    Playing,//对局中
    WaitingNewFighting,//对局结束，有玩家胜利，开始倒计时，倒计时结束后进入WaitingJoin状态
}

public class PlayerStatus
{
    public float lastActiveTime = 0;
    public bool requestDraw = false;//申请平局
   
}

public class FightingManager : MonoBehaviour
{
    

    public List<Player> players=new List<Player>();
    public int maxPlayerCount = 8;
    
    //对局状态
    public GameStatus gameStatus = GameStatus.Init;
    
    
    //UIManager
    public GameManager gameManager;
    public UIManager uiManager;
    //RoundManager 负责游戏内各种事件(比如时空震的发生时机之类的)
    public RoundManager roundManager;

    public MapManager mapManager;
    
    [Header("最大允许挂机时间")] public int kickOutTime = 120;
    [Header("玩家最大匹配时间")] public int maxWaitingPlayerTime=240;
    private float firstPlayerJoinTime = 10;

    public void Init(GameManager gameManager)
    {
        
        mapManager.Init(this);
        mapManager.PlaceAll();
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        gameStatus = GameStatus.WaitingJoin;

        
        this.gameManager = gameManager;
        uiManager = gameManager.uiManager;
        
    }
    
    public Dictionary<int,PlayerStatus> playerStatusTable=new Dictionary<int, PlayerStatus>(){};

    public void JoinGame(Player player)
    {
        if (players.Count < maxPlayerCount && !players.Contains(player) && players.Find(x=>x.uid==player.uid)==null )
        {
            players.Add(player);
            if (players.Count == 1)
            {
                firstPlayerJoinTime = Time.time;
            }
            Debug.Log("玩家"+player.userName+"加入了游戏");
            TipsDialog.ShowDialog("玩家"+player.userName+"加入了游戏",null);
            //同步UI
            uiManager.OnPlayerJoined(player);
            
            
            
            
            //游戏开始判断
            if (players.Count == 2)
            {
                TipsDialog.ShowDialog("准备完毕，对局开始", () =>
                {
                    gameStatus = GameStatus.CountDownToFight;
                    CountDownDialog.ShowDialog(3, () =>
                    {
                        TipsDialog.ShowDialog("红方先手",null);
                        gameStatus = GameStatus.Playing;
                        roundManager=new RoundManager();
                        roundManager.Init(gameManager,players);
                        
                        //两个玩家更新活跃表
                        foreach (var p in players)
                        {
                            playerStatusTable.Add(p.uid,new PlayerStatus()
                            {
                                lastActiveTime = Time.time,
                                requestDraw = false
                            });
                        }
                    });
                });
                
            }
        }
    }

    public Player GetPlayerByUid(int uid)
    {
        return players.Find(x => x.uid == uid);
    }

    
    

    private void Update()
    {
        

        var draw=true;//和棋

        //Debug.Log(gameStatus);
        if (gameStatus == GameStatus.WaitingJoin)
        {
            //Debug.Log((players.Count == 1) + "," + firstPlayerJoinTime+","+maxWaitingPlayerTime);
            if(players.Count==1 && Time.time > firstPlayerJoinTime+maxWaitingPlayerTime)
            {
                TipsDialog.ShowDialog(players[0].userName + "等待状态失效，请重新加入", () => {
                    players.Clear();
                    uiManager.ResetUi();
                });
            }
        }


        if (gameStatus == GameStatus.Playing)
        {

            for (int i = 0; i < playerStatusTable.Count; i++)
            {
                var kv = playerStatusTable.ElementAt(i);
                if (Time.time - kv.Value.lastActiveTime > kickOutTime)
                {
                    var player = GetPlayerByUid(kv.Key);
                    if (player != null)
                    {
                        Debug.Log(player.userName + "长时间未操作，踢出");
                        playerStatusTable.Clear();
                        TipsDialog.ShowDialog(player.userName + "长时间未操作，踢出", () =>
                          {
                              //var winner = FindAnotherPlayer(player.playerTeam);

                              //BattleOver(winner);
                          });
                        return;

                    }

                }

                //和棋判定
                if (kv.Value.requestDraw == false)
                {
                    draw = false;
                    continue;
                }
            }

            if (draw && playerStatusTable.Count>0)
            {
                BattleDraw();//和棋
            }
        }
    }

    void PlaceInitChess()
    {
        //foreach (var team in teams)
        //{
        //    team.PlaceInitChess(GameManager.Instance);
        //}
    }
    /// <summary>
    /// 对局结束
    /// </summary>
    /// <param name="winner"></param>
    public void BattleOver(Player winner)
    {
        roundManager.Stop();
        playerStatusTable.Clear();
        
        gameStatus =  GameStatus.WaitingNewFighting;

        BattleOverDialog.ShowDialog(15,winner, () =>
        {
            StartNewBattle();
        });
    }
    
    //和棋
    public void BattleDraw()
    {
        roundManager.Stop();
        playerStatusTable.Clear();
        
        gameStatus =  GameStatus.WaitingNewFighting;
        BattleOverDialog.ShowDialog(15,null, () =>//赢家为空时即是和棋
        {
            StartNewBattle();
        });
    }

    public void StartNewBattle()
    {
        players.Clear();
        uiManager.ResetUi();
        roundManager = null;
        
        PlaceInitChess();

        gameStatus = GameStatus.WaitingJoin;
        TipsDialog.ShowDialog("✪ ω ✪棋盘初始化完成,输入加入游戏即可游玩",null);

    }
    

    public void UpdateLastActiveTime(int uid, float time)
    {
        playerStatusTable[uid].lastActiveTime = time;
    }

    public void RequestDraw(int uid)
    {
        Player player = GetPlayerByUid(uid);
        TipsDialog.ShowDialog(player.userName+"申请和棋，双方均申请则游戏结束",null);
        playerStatusTable[uid].requestDraw = true;
    }

    private void OnDanMuReceived(string userName,int uid,string time,string text )
    {
        //找到队伍
        if (gameStatus==GameStatus.WaitingJoin && text.Split(' ')[0] == "加入"||text.Split(' ')[0] == "加入游戏")
        {
            if (gameStatus == GameStatus.WaitingJoin)
            {
                var playerAccount = BiliUserInfoQuerier.Query(uid);
                if (playerAccount.code == 0 && players.Count<maxPlayerCount)
                {
                    JoinGame(new Player(uid, userName, playerAccount.data.face,playerAccount.data.top_photo));
                }
                else
                {
                    TipsDialog.ShowDialog("Error:"+playerAccount.code+","+playerAccount.message,null);
                }
            }
        }
        
    }  
}

