using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ludiq;
using Photon.Pun;

using UnityEngine;
using Random = System.Random;

[Serializable]
public class McWeaponsPrice
{
    public string weaponName;
    public int price;
}

/// <summary>
/// 局内处理
/// </summary>
public class RoundManager : MonoBehaviour
{
    
    private float timer = 0;
    
    private GameManager gameManager;
    private FightingManager fightingManager;
    private List<Player> players=new List<Player>();

    public GameMode desireMode;//下一句玩什么模式
    //模式投票
    private int normalModeCounter = 0;
    private int battleGroundModeCounter = 0;
    public int mcModeCount = 0;
    public int marbleModeCount = 0;
    public List<int> voted=new List<int>();//只能投票一次
    
    //游戏时间
    public float elapsedTime=0;
    
   
    public void Init(GameManager gameManager, List<Player> players)
    {
        this.gameManager = gameManager;
        this.fightingManager = gameManager.fightingManager;
        this.players = players;
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.AddListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
       
        
        elapsedTime = 0;
        UnityTimer.Timer.Register(30, () =>
        {
            fightingManager.StartCoroutine(ParseGiftList());
        });
    }

    

    public void Update()
    {
        timer += Time.deltaTime;
        elapsedTime += Time.deltaTime;
        if (timer >600)
        {
            GameEnvEventManager.Instance.PlayRandomEvent();
            

            timer = 0;
        }
    }

    /// <summary>
    /// 停止游戏内回合或者说是时间处理
    /// </summary>
    public virtual void Stop()
    {
        EventCenter.RemoveListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.RemoveListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
        // 清除所有协程StopAllCoru();
        fightingManager.StopAllCoroutines();
        elapsedTime = 0;
        voted.Clear();
        players.Clear();
    }

    private void OnDestroy()
    {
        Stop();
    }

    private void OnDanMuReceived(string userName,int uid,string time,string text)
    {
        if (PhotonLauncher.playMode == PlayMode.Photon)
        {
            var photonView = GetComponent<PhotonView>();
            photonView.RPC(nameof(ParseCommand),RpcTarget.All, uid,text);
        }
       
        
        ParseCommand(uid,text);
        
        
    }

    Player GetPlayerByUid(int uid)
    {
        return players.Find(x => x.uid == uid);
    }

    protected Planet GetPlantByPlayerUid(int uid)
    {
        //return PlanetManager.Instance.allPlanets.Find(x => x.planetCommanders !=null && x.owner.uid == uid);
        for (int i = 0; i < PlanetManager.Instance.allPlanets.Count; i++)
        {
            var planet = PlanetManager.Instance.allPlanets[i];
            var commander = planet.planetCommanders.Find(x => x.uid == uid);
            if (commander!=null)
            {
                return planet;
            }
        }

        return null;
    }

    protected Planet GetPlanetByIndex(int index)
    {
        return PlanetManager.Instance.allPlanets.Find(x => x.planetIndex == index);
    }

    
    
   

    void MapVote(int uid,GameMode gameMode)
    {
        if(voted.Contains(uid))
            return;
        if (gameMode == GameMode.Normal)
            normalModeCounter++;
        if (gameMode == GameMode.BattleGround)
            battleGroundModeCounter++;
        if (normalModeCounter > battleGroundModeCounter)
            desireMode = GameMode.Normal;
        else
        {
            desireMode = GameMode.BattleGround;
        }
        voted.Add(uid);

        gameManager.uiManager.UpdateMapVoteUi(normalModeCounter,battleGroundModeCounter);
    }


    protected virtual void ParseTrim(int uid, string text,string trim)
    {
        
    }

   
    
    
    [PunRPC]
    //解析命令
    protected virtual void ParseCommand(int uid, string text)
    {
        var user = GetPlayerByUid(uid);
        var validUser = user != null;
        var planet = GetPlantByPlayerUid(uid);
        
        if (text.Equals("投票混战模式"))
        {
            MapVote(uid,GameMode.Normal);
        }
        
        if (text.Equals("投票团战模式"))
        {
            MapVote(uid,GameMode.BattleGround);
        }

        if (text.Equals("投票MC模式"))
        {
            MapVote(uid,GameMode.MCWar);
        }

        if (text.Equals("投票弹球模式"))
        {
            MapVote(uid, GameMode.Marble);
        }

        
        if (!validUser || planet==null || user.die || planet.die)
        {
            //局外人或者已经淘汰
            return;
        }
        
        //重置玩家的上次发弹幕时间

        PlanetCommander planetCommander = null;
        if (planet)
            planetCommander = planet.GetCommanderByUid(uid);

        planetCommander?.UpdateLastMsgTime(Time.time);
        
        
        
        var trim=Regex.Replace(text.Trim(), "\\s+", "");//去除所有空格
        

        ParseTrim( uid,text,trim);

    }

    
    

    public struct GiftMSg
    {
        public int uid;
        public string giftName;
        public int battery;

       
        public GiftMSg(int uid, string giftName,int battery)
        {
            this.uid = uid;
            this.giftName = giftName;
            this.battery = battery;
        }
    }
    
    public Queue<GiftMSg> giftQueue=new Queue<GiftMSg>();
    
    //送礼区域变量*****************
    
    
    public void OnGiftReceived(int uid, string userName, int num, string giftName, int totalCoin)
    {
        if (elapsedTime < 30)
        {
            TipsDialog.ShowDialog("开局30秒内投喂的礼物会在30秒后生效",null);
        }
        
        for (int i = 0; i < num; i++)
        {
            giftQueue.Enqueue(new GiftMSg(uid,giftName,totalCoin));
        }
        
    }



    protected virtual void ParseGift(int uid, string giftName, int battery)
    {
        
    }
    

    IEnumerator ParseGiftList()
    {
        while (true)
        {
            if (giftQueue.Count > 0)
            {
                var giftMsg = giftQueue.Peek();
                
                ParseGift(giftMsg.uid,giftMsg.giftName,giftMsg.battery);
                
                
                giftQueue.Dequeue();
            }

            yield return null;
        }
    }

    
}
