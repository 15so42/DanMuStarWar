using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Ludiq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Timer = System.Timers.Timer;

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
    public static FightingManager Instance;

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

    [Header("每回合加入时间")] public int waitingJoinSecond = 120;
    [Header("最大允许挂机时间")] public int kickOutTime = 120;
    [Header("玩家最大匹配时间")] public int maxWaitingPlayerTime=240;
    private float firstPlayerJoinTime = 10;

    private UnityTimer.Timer waitingJoinTimer = null;

    [Header("重启对局时清除星球和单位")] public GameObject planetRoot;
    public GameObject battleUnitRoot;

    //存档读档
    public SaveDataManager saveDataManager;
    public PlayerDataTable playerDataTable;
    public void Init(GameManager gameManager)
    {
        
        mapManager.Init(this);
        
       
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.AddListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
        
        EventCenter.AddListener(EnumEventType.OnPlanetsSpawned,SetOwners);
        
        
        this.gameManager = gameManager;
        uiManager = gameManager.uiManager;
        
        saveDataManager.LoadByJson();//异步读档
        
        
        StartWaitingJoin();
        Instance = this;

    }

    
    public void OnGiftReceived(int uid, string userName, int num,string giftName,int totalCoin)
    {
        AddPlayerDataValue(uid,"giftPoint", totalCoin / 100);
        TipsDialog.ShowDialog("感谢支持QWQ!"+userName+"获得"+ totalCoin/100+ "个礼物点",null);
        var playerInGame = GetPlayerByUid(uid);
        if (playerInGame != null)
        {
            
            uiManager.GetPlanetUiByPlayer(playerInGame)?.UpdatGiftPointUI();
        }
       

    }
    
    #region 存档
    private void Save()
    {
        saveDataManager.SaveByJson();
    }

    //开启战绩系统
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="key">"giftPoint,winCount,loseCount"</param>
    /// <param name="value"></param>
    public void AddPlayerDataValue(int uid,string key,int value)
    {
        PlayerData playerData = playerDataTable.FindByUid(uid);
        if (key == "giftPoint")
        {
            playerData.giftPoint+=value;
        }

        if (key == "winCount")
        {
            playerData.winCount += value;
        }

        if (key == "loseCount")
        {
            playerData.loseCount += value;
        }
       
        playerDataTable.UpdateByUid(uid,playerData);
        Save();
    }
    
    
    

    #endregion

    void StartWaitingJoin()
    {
        gameStatus = GameStatus.WaitingJoin;
        //开始倒计时
        waitingJoinTimer=UnityTimer.Timer.Register(waitingJoinSecond, StartBattle, (time) =>
        {
            uiManager.UpdateWaitingJoinUI(time);
        });
        EventCenter.Broadcast(EnumEventType.OnStartWaitingJoin);
    }
    
    public Dictionary<int,PlayerStatus> playerStatusTable=new Dictionary<int, PlayerStatus>(){};

    public void JoinGame(Player player)
    {
        //var ownerAblePlanets = PlanetManager.Instance.ownerAblePlanets;//可占领行星
        
        if (players.Count < maxPlayerCount && !players.Contains(player) && players.Find(x=>x.uid==player.uid)==null )
        {
            players.Add(player);
            if (players.Count == 1)
            {
                firstPlayerJoinTime = Time.time;
            }
            
            TipsDialog.ShowDialog("玩家"+player.userName+"加入了游戏",null);
           
            EventCenter.Broadcast(EnumEventType.OnPlayerJoined,player);
        }

        if (gameStatus==GameStatus.WaitingJoin && players.Count == maxPlayerCount)
        {
            waitingJoinTimer.Cancel();
            StartBattle();
        }
    }

    void StartBattle()
    {
        if (players.Count < 2)
        {
            StartWaitingJoin();
            return;//重新等待
        }
        
        
        gameStatus = GameStatus.Playing;
        mapManager.PlaceAll();//生成场景
        
        //生成星球和石头完成后，将相机父物体移动到所有星球中心，同时相机的lookAt目标更改为相机父物体
        /*Camera mainCamera=Camera.main;
        var MPCamera = mainCamera.GetComponent<MultipleTargetCamera>();
        
        MPCamera.BeginAnim();*/

        
        
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
        EventCenter.Broadcast(EnumEventType.OnBattleStart);
        
        
    }

    //玩家占领星球
    void SetOwners()
    { 
        // for (int i = 0; i < 5; i++)
        // {
        //     var index = ((8 / 5) * i) % 8;
        //     Debug.Log(""+index);
        // }
        
        UnityTimer.Timer.Register(1, () =>
        {
            var planetNum = FightingManager.Instance.maxPlayerCount;
            var playersCount = players.Count;
            //玩家依次占领星球P
            for (int i = 0; i < players.Count; i++)
            {
                var index = ((planetNum / playersCount) * i) % planetNum;
                PlanetManager.Instance.allPlanets[index].SetOwner(players[i]);
            }
        });

    }
    
    

    public Player GetPlayerByUid(int uid)
    {
        return players.Find(x => x.uid == uid);
    }


    private void Start()
    {
        
        
    }

    private void Update()
    {
        if (roundManager!=null)
        {
            roundManager.Update();
        }

        var draw=true;//和棋

        return;
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
            //SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
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
        roundManager.Stop();

        GameEnvEventManager.Instance.Init();
        gameStatus =  GameStatus.WaitingNewFighting;
        players.Clear();
        uiManager.ResetUi();
        roundManager = null;
        playerStatusTable.Clear();
        
        //mapManager.Init(this);

        // var planets = planetRoot.GetComponentsInChildren<Transform>();
        // for (int i = 0; i < planets.Length; i++)
        // {
        //     Destroy(planets[i].gameObject);
        // }
        //
        // var units = battleUnitRoot.GetComponentsInChildren<Transform>();
        // for (int i = 0; i < units.Length; i++)
        // {
        //     Destroy(units[i].gameObject);
        // }
        
        
        StartWaitingJoin();

        gameStatus = GameStatus.WaitingJoin;
        TipsDialog.ShowDialog("初始化完成，等待加入游戏",null);

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
        if (text.Split(' ')[0] == "点歌" && text.Split(' ').Length>1)
        {
            if (playerDataTable.FindByUid(uid).giftPoint > 2)
            {
                SongHime.Instance.RequestSongByName(text.Substring(text.IndexOf(' ')));
                TipsDialog.ShowDialog($"{userName}点歌成功，消耗两个礼物点",null);
                AddPlayerDataValue(uid,"giftPoint",-2);
                var playerInGame = GetPlayerByUid(uid);
                if (playerInGame!=null)
                {
                    var planetUi=uiManager.GetPlanetUiByPlayer(playerInGame);
                    if(planetUi)
                        planetUi.UpdatGiftPointUI();
                }
                
            }
            else
            {
                TipsDialog.ShowDialog("点歌需要两个礼物点",null);
            }
            
        }

        if (text=="切歌" && uid==23204263)
        {
            SongHime.Instance.NextSong();
        }
        
        if (text.Split(' ')[0] == "加入"||text.Split(' ')[0] == "加入游戏")
        {
           
                
                if (players.Count<maxPlayerCount)
                {
                    if (gameStatus == GameStatus.WaitingJoin)
                    {
                        var newPlayer = new Player(uid, userName,  "", "");
                        JoinGame(newPlayer);
                        BiliUserInfoQuerier.Instance.Query(uid,newPlayer);
                    }
                    
                    
                    if (gameStatus == GameStatus.Playing)//游戏运行中的话额外给玩家找到能占领的星球
                    {
                        for (int i = 0; i < PlanetManager.Instance.allPlanets.Count; i++)
                        {
                            var planet = PlanetManager.Instance.allPlanets[i];
                            if (planet.owner == null && Math.Abs(planet.colonyPoint) < 1 && planet.occupied==false && planet.die==false)//没有玩家且没有被占领
                            {
                                var newPlayer = new Player(uid, userName,  "", "");
                                JoinGame(newPlayer);
                                BiliUserInfoQuerier.Instance.Query(uid,newPlayer);
                                planet.SetOwner(newPlayer);
                                
                               
                                break;
                            }
                        }
                    }
                }
                else
                {
                    TipsDialog.ShowDialog("人数已满，加入失败",null);
                }
                
            
        }
        
        
        
    }

    public void GameOver(Planet planet)
    {
        BattleOverDialog.ShowDialog(15,planet.owner,StartNewBattle);
    }
}

