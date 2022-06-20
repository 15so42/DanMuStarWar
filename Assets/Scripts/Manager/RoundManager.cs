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
    //模式投票技术
    private int normalModeCounter = 0;
    private int battleGroundModeCounter = 0;
    public List<int> voted=new List<int>();//只能投票一次
    
    //游戏时间
    public float elapsedTime=0;
    
    [Header("MC武器购买列表")]
    public List<McWeaponsPrice> mcWeaponsPrices=new List<McWeaponsPrice>();
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
    public void Stop()
    {
        EventCenter.RemoveListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        EventCenter.RemoveListener<int,string,int,string,int>(EnumEventType.OnGiftReceived,OnGiftReceived);
        // 清除所有协程StopAllCoru();
        fightingManager.StopAllCoroutines();
        elapsedTime = 0;
        voted.Clear();
        players.Clear();
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
    
    void ParseGoWhere(int uid,string trim,bool escape)
    {
        
        string pattern = @"^(去){1}(\d{1,2})$";
        string escapePattern = @"^(溜){1}(\d{1,2})$";
        string letterPattern=@"^(q|Q|l|L){1}(\d{1,2})$";
        
        if (Regex.IsMatch(trim, pattern) || Regex.IsMatch(trim,escapePattern) || Regex.IsMatch(trim,letterPattern))
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
            
            
            uidPlanet.GoWhere(uid,targetIndex,escape);
            
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

  
    
    [PunRPC]
    //解析命令
    private void ParseCommand(int uid, string text)
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
        if (true)
        {
            PlanetCommander planetCommander = null;
            if(planet)
                planetCommander= planet.GetCommanderByUid(uid);

            planetCommander?.UpdateLastMsgTime(Time.time);
        }
        
        
        var trim=Regex.Replace(text.Trim(), "\\s+", "");//去除所有空格
        
       
        
        //string spattern= @"((m|M){1}\d{1}|((s|S){1}\d{1})|((y|Y){1}\d{1})|((h|H){1}\d{1}))+";
       
        
        //string sPattern= @"([mMsShHyY]{1}(\d){1})+";
        string sPattern= @"^((m|M){1}(\d{1}))+$";
        
        // if (multipleCmd && Regex.IsMatch(trim, sPattern))
        // {
        //     Debug.Log("复合命令:"+trim);
        //     if (trim.Length % 2 == 0)
        //     {
        //         for (int i = 0; i < trim.Length; i += 2)
        //         {
        //             ParseCommand(uid,trim.Substring(i,2),false);
        //         }
        //     }
        //     
        //     return;
        //     
        // }

        if (fightingManager.gameMode == GameMode.Normal || fightingManager.gameMode==GameMode.BattleGround)
        {
            
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
            
            if (fightingManager.gameMode == GameMode.Normal && text == "投降")
            {
                ParseSurrender(uid, trim);
            }
        }
        

        

        if (fightingManager.gameMode == GameMode.MCWar)
        {
            
           
            var steveCommander = planet.GetCommanderByUid(uid) as SteveCommander;
            if (steveCommander == null)
                return;
            
            if (trim.StartsWith("去") || trim.StartsWith("q")||trim.StartsWith("Q"))
            {
                ParseGoWhere(uid, trim,false);
            }

            if (trim.StartsWith("溜") || trim.StartsWith("l")||trim.StartsWith("L"))
            {
                ParseGoWhere(uid,trim,true);
            }

            // if (trim == "复活")
            // {
            //     ParseRespawn(uid,false);
            // }
            
            if (trim == "抽取武器")
            {
                ParseRandomWeapon(steveCommander);
            }

            if (trim.StartsWith("购买"))
            {
                ParseBuyMcWeapon(steveCommander,trim);
            }

            if (trim.Equals("查询礼物点数"))
            {
                var player = steveCommander.player;
                MessageBox._instance.AddMessage("系统", player.userName+"的礼物点数为"+player.userSaveData.giftPoint);
            }

            if (trim.Equals("查询指定附魔次数"))
            {
                var player = steveCommander.player;
                MessageBox._instance.AddMessage("系统", player.userName+"剩余指定附魔次数为"+steveCommander.leftSpecificSpell);
            }
            
            if (trim.Equals("查询统计"))
            {
                var player = steveCommander.player;
                MessageBox._instance.AddMessage("系统", player.userName+
                                                      "统计信息为：\n"+"胜/败:"+player.userSaveData.winCount+"/"+player.userSaveData.loseCount+
                                                      "\n击杀/死亡:"+player.userSaveData.killCount+"/"+player.userSaveData.dieCount);
            }

            if (trim == "附魔"||trim=="随机附魔")
            {
                ParseRandomSpell(steveCommander,false,false);
            }
            else
            {
                if (trim.StartsWith("附魔"))
                {
                    ParseSpecificSpell(steveCommander, false, trim);
                }
            }

            if (trim.StartsWith("祛魔"))
            {
                ParseRemoveSpell(steveCommander,trim);
            }

            if (trim == "投降")
            {
                ParseSurrenderInMc(steveCommander);   
            }

            if (trim == "维修")
            {
                ParseFixWeapon(steveCommander);
            }

            if (trim == "兑换生命")
            {
                ParseAddMaxHp(steveCommander,false);
            }
            
            
            
            //MessageBox._instance.AddMessage("["+user.userName+"]:"+trim);
            LogTip(steveCommander,trim);
        }

    }

    void ParseCameraFocus(int uid)
    {
        var planet = GetPlantByPlayerUid(uid);
        if(planet==null)
            return;
        var commander = planet.GetCommanderByUid(uid);
        if(commander==null)
            return;
                
        var battleUnits = (commander as SteveCommander)?.battleUnits;
        if(battleUnits==null)
            return;
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] && battleUnits[i].die==false)
            {
                Camera.main.GetComponent<MCCamera>().SetTarget(battleUnits[i]);
                break;
            }
        }
    }

    void ParseBuyMcWeapon(SteveCommander steveCommander,string trim)
    {
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;

        var weaponName = trim.Substring(2);
        Debug.Log("购买武器"+weaponName);

        var pricePair = mcWeaponsPrices.Find(x => x.weaponName == weaponName);
        if (pricePair==null)
        {
            var allWeaponStr = "";
            for (int i = 0; i < mcWeaponsPrices.Count; i++)
            {
                allWeaponStr += " " + mcWeaponsPrices[i].weaponName + " ";
            }
            MessageBox._instance.AddMessage(steveCommander.player.userName,"购买命令错误，可以购买的武器有:"+allWeaponStr);
            return;
        }

        var price = pricePair.price;
        if (steveCommander.point < price)
        {
            steveCommander.commanderUi.LogTip("需要点数:"+price);
            return;
        }

        validSteve.OnBuyWeaponSuccess(weaponName);
        steveCommander.AddPoint(-1*price);

    }

    void ParseRespawn(int uid,bool byGift)
    {
        var planet = GetPlantByPlayerUid(uid);
        if(planet==null)
            return;
        var steveCommander = planet.GetCommanderByUid(uid) as SteveCommander;
        if(steveCommander==null)
            return;
        
        if (!byGift)
        {
            if (steveCommander.point < 10)
            {
                steveCommander.commanderUi.LogTip("需要点数:10");
                return;
            }
        }
        if (steveCommander.die == false)
        {
            steveCommander.commanderUi.LogTip("未死亡");
            return;
        }
        
            
        steveCommander.RespawnImmediately();
        steveCommander.AddPoint(-10);

    }

    void ParseRandomWeapon(SteveCommander steveCommander)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;

        validSteve.RandomWeapon();
        
    }
    
    public void LogTip(SteveCommander steveCommander,string trim)
    {
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;

        validSteve.LogTip(trim);
    }

    void ParseSpecificSpell(SteveCommander steveCommander, bool rare,string trim)
    {
         var validSteve = steveCommander.FindFirstValidSteve();
         if (!validSteve)
             return;

         if (steveCommander.point < 10)
         {
            
                 steveCommander.commanderUi.LogTip("需要点数:10");
                 return;
             
         }

         var spellName = trim.Substring(2);
         if (validSteve.TrySpecificSpell(spellName))
         {
             var success=validSteve.SpecificSpell(rare, spellName);
             if (success)
             {
                 steveCommander.AddPoint(-10);
                 steveCommander.leftSpecificSpell--;
             }
         }
    }

    void ParseSurrenderInMc(SteveCommander steveCommander)
    {
        steveCommander.ParseSurrenderInMc();
    }
    
    void ParseRandomSpell(SteveCommander steveCommander,bool rare,bool byGift)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;
        
        if (steveCommander.point < 8 )
        {
            if (!byGift)
            {
                steveCommander.commanderUi.LogTip("需要点数:8");
                return;
            }
           
        }

        if (validSteve.TryRandomSpell(byGift))
        {
            
            validSteve.RandomSpell(rare,byGift);
            if (!byGift)
            {
                steveCommander.AddPoint(-8);
            }
            
        }
            
      
    }

    void ParseRemoveSpell(SteveCommander steveCommander,string trim)
    {
        
        var pattern=@"^(祛魔){1}(\d{1})$";
        if(Regex.IsMatch(trim,pattern)==false)
            return;

        var index = trim.Substring(2);
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;

        if (steveCommander.point <0)
        {
            steveCommander.commanderUi.LogTip("需要点数:0");
            return;
        }
        
        if(validSteve.RemoveSpell(int.Parse(index)))
        {
            //steveCommander.AddPoint(-3);
        }
        
       
        
    }
    
    void ParseAddMaxHp(SteveCommander steveCommander,bool byGift)
    {
        if (byGift)
        {
            steveCommander.desireMaxHp ++ ;
        }
       
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;
        if(byGift)
            validSteve.AddMaxHp(3);
        else
        {
            if (steveCommander.point < 8)
            {
                steveCommander.commanderUi.LogTip("需要点数:8");
                return;
            }
            validSteve.AddMaxHp(5);
            steveCommander.AddPoint(-8);
        }
    }
    
    void ParseFixWeapon(SteveCommander steveCommander)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;


        if (steveCommander.point < 5)
        {
            steveCommander.commanderUi.LogTip("需要点数:5");
            return;
        }

        validSteve.FixWeapon(20);
        steveCommander.AddPoint(-5);
        
    }
    
    void ParseRareWeapon(SteveCommander steveCommander)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;
        
        validSteve.RandomRareWeapon();
    }
    
    void ParseAddPoint(SteveCommander steveCommander)
    {
        steveCommander.AddPoint(1.5f);
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
    

    void ParseGiftInMcWar(int uid, string giftName,int battery)
    {
        var planet = GetPlantByPlayerUid(uid);
        if(planet==null)
            return;
        var steveCommander = planet.GetCommanderByUid(uid) as SteveCommander;
        if(steveCommander==null)
            return;
        
        // if (giftName == "小花花" || giftName=="辣条")
        // {
        //     ParseCameraFocus(uid);
        // }

        if (giftName == "打call")
        {
            //ParseRespawn(uid,true);
            //ParseAddMaxHp(steveCommander,true);
            ParseRandomSpell(steveCommander,false,true);
        }

        if (giftName == "这个好诶")
        {
            //ParseRespawn(uid,true);
            //ParseAddMaxHp(steveCommander);
            
            
            //特殊武器
            ParseRareWeapon(steveCommander);
        }
        
        // if (giftName == "白银宝盒")
        // {
        //     //特殊武器
        //     ParseRareWeapon(steveCommander);
        // }

        if (giftName == "牛哇牛哇" || giftName == "牛哇")
        {
            ParseAddPoint(steveCommander);
        }
        
        if (giftName == "flag")
        {
            ParseAddMaxHp(steveCommander,true);
        }
        //Debug.LogError(battery+","+battery/100);

        if (giftName == "辣条")
        {
            battery = 0;
        }

        if (steveCommander.flowerSpell == false && battery>0)
        {
            steveCommander.leftSpecificSpell++;
            MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"通过电池礼物获得1次额外指定附魔次数（每局任意电池礼物可获得一次额外指定附魔次数，每局限一次）");
            steveCommander.flowerSpell = true;
        }
        
        EventCenter.Broadcast(EnumEventType.OnMcBatteryReceived,planet,battery==0? 0:battery/100);
    }

    void ParseGift(int uid,string giftName)
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

    IEnumerator ParseGiftList()
    {
        while (true)
        {
            if (fightingManager.gameMode == GameMode.MCWar)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(10);
            }
            
            if (giftQueue.Count > 0)
            {
                var giftMsg = giftQueue.Peek();
                
                if (fightingManager.gameMode == GameMode.MCWar)
                {
                    
                    ParseGiftInMcWar(giftMsg.uid,giftMsg.giftName,giftMsg.battery);
                }
                else
                {
                    ParseGift(giftMsg.uid,giftMsg.giftName);
                }
                
                
                giftQueue.Dequeue();
            }
        }
    }

    
}
