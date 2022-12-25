using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using Ludiq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = System.Object;
using Timer = System.Timers.Timer;

public enum GameStatus
{
    Init,//程序初始化状态
    WaitingJoin,//等待玩家加入
    CountDownToFight,//玩家加入完毕后倒计时
    Playing,//对局中
    WaitingNewFighting,//对局结束，有玩家胜利，开始倒计时，倒计时结束后进入WaitingJoin状态
}





public enum GameMode
{
    Normal,
    BattleGround,
    MCWar,
    McPve,
    Marble
}

public class FightingManager : MonoBehaviourPunCallbacks
{
    public static FightingManager Instance;

    public List<Player> players=new List<Player>();
    public int maxPlayerCount = 8;
    
    //局内退出玩家
    [HideInInspector]public List<int> exitPlayers=new List<int>();
    
    //对局状态
    public GameStatus gameStatus = GameStatus.Init;

    [Header("游戏模式")]
    public GameMode gameMode;
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

  

   
    
    //服务器存档
    public NetSaveDataManager saveDataManager;
    public PlayerDataTable playerDataTable;

    public ColorTable colorTable;

    [Header("初始化星球配置")] public int firstPlanetIndex = 0;
    public int lastPlanetIndex = 11;
    

    public MCPosManager mcPosManager;
    
    
    //游戏是否联网
    public PlayMode photonPlayMode;
    public List<Player> photonPlayers=new List<Player>();
    
    public void Init(GameManager gameManager,PlayMode playMode)
    {
        this.photonPlayMode = playMode;
        mapManager.Init(this);
        
       
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.AddListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
        
        EventCenter.AddListener(EnumEventType.OnPlanetsSpawned,SetOwners);
        
        
        this.gameManager = gameManager;
        uiManager = gameManager.uiManager;
        
        saveDataManager=new NetSaveDataManager();
        
        
        StartWaitingJoin();
        Instance = this;

        OnPlayerLoadedScene();
        
    }
    
    

    
    public void OnGiftReceived(int uid, string userName, int num,string giftName,int totalCoin)
    {
        AddPlayerDataValue(uid,"giftPoint", totalCoin / 100);
        //TipsDialog.ShowDialog("感谢支持QWQ!"+userName+"获得"+ totalCoin/100+ "个礼物点",null);
        TipsDialog.ShowDialog("感谢"+userName+"的支持QWQ!",null);
        var playerInGame = GetPlayerByUid(uid);
        if (playerInGame != null)
        {
            
            uiManager.GetPlanetUiByPlayer(playerInGame)?.UpdatGiftPointUI();
        }
       

    }
    
    #region 存档
   

    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="key">"giftPoint,winCount,loseCount"</param>
    /// <param name="value"></param>
    public void AddPlayerDataValue(int uid,string key,object value)
    {
        var findRet = players.Find(x => x.uid == uid);
        if(findRet==null)
            return;
        
        UserSaveData userSaveData = findRet.userSaveData;
        if(userSaveData==null)//特殊情况容错处理
           return;
        
        if (key == "giftPoint")
        {
            userSaveData.giftPoint+=(int)value;
        }

        if (key == "coin")
        {
            userSaveData.coin += (int)value;
        }

        if (key == "winCount")
        {
            userSaveData.winCount += (int)value;
        }

        if (key == "loseCount")
        {
            userSaveData.loseCount += (int)value;
        }

        if (key == "killCount")
        {
            userSaveData.killCount += (int)value;
        }

        if (key == "dieCount")
        {
            userSaveData.dieCount += (int)value;
        }

        if (key == "skinId")
        {
            userSaveData.skinId = (int)value;
        }
       
       
    }
    
    
    

    #endregion


    public void SyncWatingStartByRPC()
    {
        photonView.RPC(nameof(SyncWaitingStart),RpcTarget.All);
    }

    [PunRPC]
    public void SyncWaitingStart()
    {
        waitingJoinTimer.Cancel();
        StartWaitingJoin();
    }
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

    private void OnDestroy()
    {
        EventCenter.RemoveListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.RemoveListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
        EventCenter.RemoveListener(EnumEventType.OnPlanetsSpawned,SetOwners);
    }

    public void JoinGameByChoose(string teamName,int uid,string userName)
    {
        Player player=new Player(uid,userName,"","");
        BiliUserInfoQuerier.Instance.Query(uid,player);
        
        var targetPlanet = PlanetManager.Instance.allPlanets[teamName=="加入黄队"?firstPlanetIndex: lastPlanetIndex ];
        int uiArea = teamName == "加入黄队" ? 0 : 1;
        PlanetCommander commander=new SteveCommander(player.uid,player,targetPlanet.planetColor);
        if (players.Count < maxPlayerCount  &&
            players.Find(x => x.uid == player.uid) == null)
        {
            players.Add(player);
            targetPlanet.AddCommander(commander,uiArea);
            EventCenter.Broadcast(EnumEventType.OnPlayerJoined,player);
            TipsDialog.ShowDialog(userName+"加入了游戏",null);
        }
        else
        {
            TipsDialog.ShowDialog("人数已满，无法加入",null);
        }
        
        player.onSetUserData+= () =>
        {
            if (player.userSaveData.jianzhang == 0)
            {
                TipsDialog.ShowDialog("仅舰长可使用选队功能",null);
                commander.OnHangUp();
                return;
            }
            // var targetPlanet = PlanetManager.Instance.allPlanets[teamName=="加入黄队"?firstPlanetIndex: lastPlanetIndex ];
            // int uiArea = teamName == "加入黄队" ? 0 : 1;
            // PlanetCommander commander=new SteveCommander(player.uid,player,targetPlanet.planetColor);
            // if (players.Count < maxPlayerCount  &&
            //     players.Find(x => x.uid == player.uid) == null)
            // {
            //     players.Add(player);
            //     targetPlanet.AddCommander(commander,uiArea);
            //     EventCenter.Broadcast(EnumEventType.OnPlayerJoined,player);
            //     TipsDialog.ShowDialog(userName+"加入了游戏",null);
            // }
            // else
            // {
            //     TipsDialog.ShowDialog("人数已满，无法加入",null);
            // }

            
            
        };
    }
    
   
    
    public void JoinGame(Player player)
    {
        
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

    }

    public void StartBattleByPhoton()
    {
        photonView.RPC(nameof(JoinAllPlayer),RpcTarget.All);
        //photonView.RPC(nameof(StartBattle),RpcTarget.All);
        
    }

    [PunRPC]
    void JoinAllPlayer()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var player=new Player(PhotonNetwork.PlayerList[i].ActorNumber,PhotonNetwork.PlayerList[i].NickName,"","");
            JoinGame(player);
        }
    }

    public int minPlayer = 2;
    [PunRPC]
    public void StartBattle()
    {
        if (players.Count < minPlayer && photonPlayMode==PlayMode.Live)
        {
            StartWaitingJoin();
            return;//重新等待
        }
        
        mapManager.PlaceAll();//生成场景
        
        roundManager.Init(gameManager,players);
                        
        //两个玩家更新活跃表
      
        EventCenter.Broadcast(EnumEventType.OnBattleStart);
        
        uiManager.OpenTimer();
        MessageBox._instance.Show();
    }

    protected virtual void SetOwnersAfter1Second()
    {
        gameStatus = GameStatus.Playing;
    }
    
    //玩家占领星球
    void SetOwners()
    {
        UnityTimer.Timer.Register(1, ()=>
        {
            SetOwnersAfter1Second();
            McRankDialog.ShowDialog();
        });
       
    }
    
    

    public Player GetPlayerByUid(int uid)
    {
        return players.Find(x => x.uid == uid);
    }


    protected void Start()
    {
        //MapChanger.Instance.ChangeMap(MapChanger.Instance.desireMap);
    }

    

 

    public void StartNewBattle()
    {
        roundManager.Stop();

        GameEnvEventManager.Instance.Init();
        gameStatus =  GameStatus.WaitingNewFighting;
        players.Clear();
        uiManager.ResetUi();
        
       
        
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



    public virtual void OnJoinPlayingDanMuReceived(Player toJoinPlayer)
    {
       
               
    }

   

    private void OnDanMuReceived(string userName,int uid,string time,string text )
    {
        if (text.Split(' ')[0] == "点歌" && text.Split(' ').Length>1)
        {
            return;

        }

        // if (text=="切歌" && uid==23204263)
        // {
        //     SongHime.Instance.NextSong();
        // }

        //if (text.Equals("加入黄队") || text.Equals("加入绿队") && gameStatus==GameStatus.Playing)
        //{
        //    JoinGameByChoose(text,uid,userName);
        //}
        
        if (text.Split(' ')[0] == "加入"||text.Split(' ')[0] == "加入游戏")
        {
           
                

                if (players.Count >= maxPlayerCount)
                {
                    TipsDialog.ShowDialog("人数已满",null);
                    
                }
                else
                {
                    if (gameStatus == GameStatus.WaitingJoin)
                    {
                        var newPlayer = new Player(uid, userName,  "", "");
                        JoinGame(newPlayer);
                        BiliUserInfoQuerier.Instance.Query(uid,newPlayer);
                    }

                    if (gameStatus == GameStatus.Playing)
                    {
                        var newPlayer = new Player(uid, userName,  "", "");
                        if (players.Find(x => x.uid == newPlayer.uid) != null)//已经加入
                            return;
                        OnJoinPlayingDanMuReceived(newPlayer);
                    }
                    
                }


        }
        
        
        
    }

    
    public void OnPlayerLoadedScene()
    {
        // var player=new Player(photonPlayers.Count,PhotonNetwork.NickName,"","");
        // photonPlayers.Add(player);
        // JoinGame(player);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"battleSceneLoaded",true} });
    }

    

    public void GameOver(Planet planet,GameMode newMode)
    {
        gameStatus = GameStatus.WaitingNewFighting;
        MessageBox._instance.Hide();
        
        BattleOverDialog.ShowDialog(15,planet.owner,planet.planetCommanders,
            ()=>
            {
                gameMode = newMode;
                if (newMode == GameMode.BattleGround)
                    maxPlayerCount = 24;
                if (newMode == GameMode.Normal)
                    maxPlayerCount = 6;
                StartNewBattle();
            });
        OnGameOver();
    }

    protected virtual void OnGameOver()
    {
        UnityTimer.Timer.Register(10, () =>
        {
            MapChanger.Instance.ChangeMap(MapChanger.Instance.desireMap);
            MapChanger.Instance.ClearVoted();
        });

    }

    public void GameOverByMc(List<PlanetCommander> winners,List<PlanetCommander> losers,bool uploadRank,bool pveMode=false)
    {
        gameStatus = GameStatus.WaitingNewFighting;
        MessageBox._instance.Hide();


        var emeraldCount = 0f;
            if (winners!=null && winners.Count > 0)
            {
                for (int i = 0; i < winners.Count; i++)
                {
                    var player = winners[i].player;
                    if (uploadRank)
                    {
                        AddPlayerDataValue(player.uid, "winCount", 1);
                    }

                    if (pveMode)
                    {
                        AddPlayerDataValue(player.uid, "coin", PVEManager.Instance.difficulty);
                        
                        emeraldCount = PVEManager.Instance.difficulty;
                    }
                }
            }

            if (losers != null && losers.Count > 0)
            {
                for (int i = 0; i < losers.Count; i++)
                {
                    var player = losers[i].player;
                    if (uploadRank)
                    {
                        AddPlayerDataValue(player.uid, "loseCount", 1);
                    }

                    if (pveMode)
                    {
                        AddPlayerDataValue(player.uid, "coin", (int) (PVEManager.Instance.difficulty / 2));
                        
                        emeraldCount = (int) (PVEManager.Instance.difficulty / 2);
                    }
                }
            }
            
        
        exitPlayers.Clear();
        
        SaveAllPlayer();
        MCBattleOverDialog.ShowDialog(15,GameMode.MCWar,winners,losers,
            ()=>
            {
                StartNewBattle();
            });
        EmeraldGiftDialog.ShowDialog((int) emeraldCount);
        OnGameOver();
        EventCenter.Broadcast(EnumEventType.OnBattleOver);
    }

    public void SaveAllPlayer()
    {
        saveDataManager.Save(players);
    }
}

