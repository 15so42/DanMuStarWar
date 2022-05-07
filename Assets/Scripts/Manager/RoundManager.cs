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
    private FightingManager fightingManager;
    private List<Player> players=new List<Player>();

    public GameMode desireMode;//下一句玩什么模式
    //模式投票技术
    private int normalModeCounter = 0;
    private int battleGroundModeCounter = 0;
    public List<int> voted=new List<int>();//只能投票一次
    public void Init(GameManager gameManager, List<Player> players)
    {
        this.gameManager = gameManager;
        this.fightingManager = gameManager.fightingManager;
        this.players = players;
        EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.AddListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
        fightingManager.StartCoroutine(ParseGiftList());
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
        // 清除所有协程StopAllCoru();
        fightingManager.StopAllCoroutines();
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

    Planet GetPlanetByIndex(int index)
    {
        return PlanetManager.Instance.allPlanets.Find(x => x.planetIndex == index);
    }

    void ParseClaimWar(int uid,string trim)
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
    
    void ParseRecall(int uid,string trim)
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

    void ParseRecallAll(int uid)
    {
        var uidPlanet = GetPlantByPlayerUid(uid);
        if(uidPlanet==null)
            return;
        uidPlanet.RecallAll(uid);
    }
    
    void ParseGather(int uid,string trim)
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

    void ParseDefend(int uid, string trim)
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

    void ParseComplexCommand(int uid, string trim)
    {
        string pattern= @"((m|M){1}\d{1}|(s|S){1}\d{1}|(y|Y){1}\d{1}|(h|H){1}\d{1})+";
        
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
            if (planet)
            {
                
                    planet.ChangeSkill(uid,skillIndex);
                
            }
           
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
            if(planet)
                planet.UseSkill(uid,skillIndex);
            
           
        }
    }
    
    void ParseBuySkill(int uid,string trim)//使用技能1或者u1或者U1
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
    
    void ParseRemoveSkill(int uid,string trim)
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
    
    void ParseRollSkill(int uid,string trim)
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
    
    void ParseShowSkillDesc(int uid,string trim)
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
    
    void ParseUrgentRepair(int uid,string trim)
    {
        if (trim == "紧急维修")
        {
            Debug.Log("解析紧急维修命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.UrgentRepair(uid);
        }
    }
    
    void ParseCloseAutoRoll(int uid,string trim)
    {
        if (trim == "关闭自动抽卡")
        {
            Debug.Log("解析关闭自动抽卡命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.autoRoll=false;
        }
    }
    
    void ParseOpenAutoRoll(int uid,string trim)
    {
        if (trim == "开启自动抽卡")
        {
            Debug.Log("解析开启自动抽卡命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if(planet)
                planet.autoRoll=true;
        }
    }
    
    void ParseSurrender(int uid,string trim)
    {
        if (trim == "投降")
        {
            Debug.Log("解析投降命令:"+trim);
           
            var planet=GetPlantByPlayerUid(uid);
            if (planet)
                planet.Die();
        }
    }

    void ParseShowWhere(int uid)
    {
        var planet=GetPlantByPlayerUid(uid);
        if (planet)
            planet.ShowCommanderPosition(uid); 
    }
    
    void ParseGoWhere(int uid,string trim)
    {
        
        string pattern = @"^(去){1}(\d{1,2})$";
        
        if (Regex.IsMatch(trim, pattern))
        {
           
            var subStringCount = 1;
            if(Regex.IsMatch(trim, @"\d{2}$"))
            {
                subStringCount = 2;
            }
            int targetIndex = Int32.Parse(trim.Substring(trim.Length-subStringCount,subStringCount));
            Debug.Log("解析去命令:"+targetIndex);
            var uidPlanet = GetPlantByPlayerUid(uid);
            if(uidPlanet==null)
                return;
            
            
            uidPlanet.GoWhere(uid,targetIndex);
            
        }
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
    
    //解析命令
    private void ParseCommand(int uid, string text,bool multipleCmd=true)
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

        
        if (!validUser || planet==null || user.die || planet.die)
        {
            //局外人或者已经淘汰
            return;
        }
        
        //重置玩家的上次发弹幕时间
        if (fightingManager.gameMode == GameMode.BattleGround || fightingManager.gameMode == GameMode.Normal)
        {
            PlanetCommander planetCommander = null;
            if(planet)
                planetCommander= planet.GetCommanderByUid(uid);

            planetCommander?.UpdateLastMsgTime(Time.time);
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
        
        //string spattern= @"((m|M){1}\d{1}|((s|S){1}\d{1})|((y|Y){1}\d{1})|((h|H){1}\d{1}))+";
       
        
        //string sPattern= @"([mMsShHyY]{1}(\d){1})+";
        string sPattern= @"^((m|M){1}(\d{1}))+$";
        
        if (multipleCmd && Regex.IsMatch(trim, sPattern))
        {
            Debug.Log("复合命令:"+trim);
            if (trim.Length % 2 == 0)
            {
                for (int i = 0; i < trim.Length; i += 2)
                {
                    ParseCommand(uid,trim.Substring(i,2),false);
                }
            }
            
            return;
            
        }

        if (fightingManager.gameMode == GameMode.Normal || fightingManager.gameMode==GameMode.BattleGround)
        {
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

           
        }
        

        if (text == "紧急维修")
        {
            ParseUrgentRepair(uid,trim);
        }

        if (fightingManager.gameMode == GameMode.MCWar)
        {
            if (trim.StartsWith("去"))
            {
                ParseGoWhere(uid, trim);
            }
        }
      
        
        
        if (text.StartsWith("技能说明"))
        {
            ParseShowSkillDesc(uid, trim);
        }
        
        if (text.Equals("我在哪"))
        {
            ParseShowWhere(uid);
        }
        
        if (text.Equals("我在哪"))
        {
            ParseShowWhere(uid);
        }

       
        
        if (fightingManager.gameMode == GameMode.Normal && text == "投降")
        {
            ParseSurrender(uid, trim);
        }
        
        
    }

    public struct GiftMSg
    {
        public int uid;
        public string giftName;

        public GiftMSg(int uid, string giftName)
        {
            this.uid = uid;
            this.giftName = giftName;
        }
    }
    
    public Queue<GiftMSg> giftQueue=new Queue<GiftMSg>();
    
    //送礼区域变量*****************
    
    
    public void OnGiftReceived(int uid, string userName, int num, string giftName, int totalCoin)
    {

        for (int i = 0; i < num; i++)
        {
            giftQueue.Enqueue(new GiftMSg(uid,giftName));
        }
        
    }

    void ParseGift(int uid,string giftName)
    {
        if (giftName == "小花花")
        {
            ParseCommand(uid, "c",true);
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

    IEnumerator ParseGiftList()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            if (giftQueue.Count > 0)
            {
                var giftMsg = giftQueue.Peek();
                ParseGift(giftMsg.uid,giftMsg.giftName);
                giftQueue.Dequeue();
            }
        }
    }

    
}
