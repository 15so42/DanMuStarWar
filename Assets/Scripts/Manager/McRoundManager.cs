using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameCode.Tools;
using Photon.Pun;
using UnityEngine;



public class McRoundManager : RoundManager
{
    [Header("MC武器购买列表")]
    public List<McWeaponsPrice> mcWeaponsPrices=new List<McWeaponsPrice>();
    public List<McWeaponsPrice> rareWeaponPrices=new List<McWeaponsPrice>();


    public Dictionary<string,int> votedMap=new Dictionary<string, int>()
    {
        {"村庄",0},{"矿井",0},{"PVE",0}
    };


    void ShowShopList()
    {
        MessageBox._instance.AddMessage("系统","当前可交易物品有圣诞树(200绿宝石)，附魔槽位(120绿宝石),自爆羊(800,仅PVE模式),稀有武器(400)，红包(100),托管者(50),更多物品请等待后续版本添加");
    }

    protected override void ParseTrim(int uid, string text, string trim)
    {
        base.ParseTrim(uid, text, trim);
        var planet = GetPlantByPlayerUid(uid);
         var steveCommander = planet.GetCommanderByUid(uid) as SteveCommander;
            if (steveCommander == null)
                return;
            
            if (trim.StartsWith("去") || trim.StartsWith("q")||trim.StartsWith("Q"))
            {
                ParseGoWhere(uid, trim,false);
            }

            if (trim.Equals("驻守"))
            {
                ParseGuard(uid);
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

            if (trim.StartsWith("召唤"))
            {
                ParseSummon(steveCommander,trim);
            }
            if (trim.StartsWith("律令"))
            {
                ParseGatherSummons(steveCommander,trim);
            }

            if (trim.StartsWith("购买"))
            {
                ParseBuyMcWeapon(steveCommander,trim);
            }


            
            if (trim.StartsWith("交易"))
            {
                if (trim.Equals("交易列表"))
                {
                    ShowShopList();
                }
                else
                {
                    ParseShop(steveCommander,trim);
                }
            }
            
            if (trim.StartsWith("兑换") && trim.StartsWith("兑换生命")==false)
            {
                ParseGiftWeapon(steveCommander, trim);
            }

            if (trim.Equals("查询礼物点数"))
            {
                var player = steveCommander.player;
                if (player.userSaveData == null)
                {
                    MessageBox._instance.AddMessage("系统", "查询礼物点数失败，数据服务器暂时不可用");
                    return;
                }
                MessageBox._instance.AddMessage("系统", player.userName+"的礼物点数为"+player.userSaveData.giftPoint);
            }
            
            if (trim.Equals("查询绿宝石"))
            {
                var player = steveCommander.player;
                if (player.userSaveData == null)
                {
                    MessageBox._instance.AddMessage("系统", "查询绿宝石失败，数据服务器暂时不可用");
                    return;
                }
                MessageBox._instance.AddMessage("系统", player.userName+"的绿宝石点数为"+player.userSaveData.coin);
            }

            if (trim.Equals("查询指定附魔次数"))
            {
                var player = steveCommander.player;
                MessageBox._instance.AddMessage("系统", player.userName+"剩余指定附魔次数为"+steveCommander.leftSpecificSpell);
            }
            
            if (trim.Equals("查询统计"))
            {
                var player = steveCommander.player;
                if (player.userSaveData == null)
                {
                    MessageBox._instance.AddMessage("系统", "查询统计失败，数据服务器暂时不可用");
                    return;
                }
                MessageBox._instance.AddMessage("系统", player.userName+
                                                      "统计信息为：\n"+"胜/败:"+player.userSaveData.winCount+"/"+player.userSaveData.loseCount+
                                                      "\n击杀/死亡:"+player.userSaveData.killCount+"/"+player.userSaveData.dieCount);
            }

            if (trim.StartsWith("多次附魔"))
            {
                var countStr = trim.Substring(4);
                int count = 0;
                try
                {
                    count = long.Parse(countStr)>25?25:int.Parse(countStr);
                }
                catch (Exception e)
                {
                    return;
                }
                
                
                for (int i = 0; i < count; i++)
                {
                    ParseRandomSpell(steveCommander,false,false);
                }
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

            if (trim == "关闭自动维修")
            {
                steveCommander.autoFixWeapon = false;
            }

            if (trim == "开启自动维修")
            {
                steveCommander.autoFixWeapon = true;
            }

            if (trim == "查询角色状态")
            {
                var steve = steveCommander.FindFirstValidSteve();
                if (steve)
                {
                    var weapon = steve.GetActiveWeapon();
                    
                    var str = "";
                    var weaponNbt = weapon.weaponNbt;
                    for (int i = 0; i < weaponNbt.enhancementLevels.Count; i++)
                    {
                        if (weaponNbt.enhancementLevels[i].level>0)
                        {
                            str += "|" + weaponNbt.enhancementLevels[i].enhancementName+weaponNbt.enhancementLevels[i].level;
                        }
                    }
                    MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"状态：\n血量:"+steve.props.hp+"/"+steve.props.maxHp+"\n"+
                                                         "护盾："+steve.props.shield+"/"+steve.props.maxShield+"\n"+
                                                         "武器耐久："+weapon.endurance+"/"+weapon.maxEndurance+"\n"+
                                                         "附魔信息:"+weapon.weaponName+"|"+ str);
                }
            }

            if (trim.StartsWith("多次兑换生命"))
            {
                var countStr = trim.Substring(6);
                int count = 0;
                try
                {
                    
                    count = long.Parse(countStr)>25?25:int.Parse(countStr);
                }
                catch (Exception e)
                {
                    return;
                }
                for (int i = 0; i < count; i++)
                {
                    ParseAddMaxHp(steveCommander,false);
                }
            }

            if (trim == "兑换生命")
            {
                ParseAddMaxHp(steveCommander,false);
            }

            // if (trim.StartsWith("换地图"))
            // {
            //     var sceneName = trim.Substring(3);
            //     if (votedMap.ContainsKey(sceneName))
            //     {
            //         votedMap[sceneName]++;
            //         var maxKv = votedMap.ElementAt(0);
            //         foreach (var kv in votedMap)
            //         {
            //             if (kv.Value > maxKv.Value)
            //             {
            //                 maxKv = kv;
            //             }
            //         }
            //
            //         if (FightingManager.Instance as McFightingManager)
            //         {
            //             (FightingManager.Instance  as McFightingManager)?.SetNextMap(maxKv.Key);
            //         }
            //
            //         if (FightingManager.Instance as McPveFightingManager)
            //         {
            //             (FightingManager.Instance  as McPveFightingManager)?.SetNextMap(maxKv.Key);
            //         }
            //         
            //         
            //         
            //         MessageBox._instance.AddMessage("经过投票决定下一局地图为："+maxKv.Key);
            //     }
            // }
            
            
            //MessageBox._instance.AddMessage("["+user.userName+"]:"+trim);
            LogTip(steveCommander,trim);
    }

    public override void Stop()
    {
        base.Stop();
        
        for (int i=0;i<votedMap.Keys.Count;i++)
        {
            votedMap[votedMap.Keys.ElementAt(i)] = 0;
        }
    }

    protected void ParseGoWhere(int uid,string trim,bool escape)
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
            long targetIndex = long.Parse(trim.Substring(trim.Length-subStringCount,subStringCount));
            Debug.Log("解析去命令:"+targetIndex);
            if(targetIndex>1000)
                return;
            var uidPlanet = GetPlantByPlayerUid(uid);
            if(uidPlanet==null)
                return;
            
            
            uidPlanet.GoWhere(uid,(int)targetIndex,escape);
            
        }
    }

    protected void ParseGuard(int uid)
    {
        var uidPlanet = GetPlantByPlayerUid(uid);
        if(uidPlanet==null)
            return;
            
            
        uidPlanet.Guard(uid);
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


    void ParseSummon(SteveCommander steveCommander, string trim)
    {
        var steve = steveCommander.FindFirstValidSteve();
        if (!steve)
        {
            return;
        }

        var handWeapon = steve.GetActiveWeapon();
        if (handWeapon!=null && handWeapon.weaponName == "召唤法杖")
        {
            var monsterName = trim.Substring(2);
            
            handWeapon.Summon(monsterName);
        }
    }
    void ParseShop(SteveCommander steveCommander, string trim)
    {
        if (steveCommander.player.userSaveData == null)
        {
            return;
        }
        if (trim == "交易圣诞树")
        {
            if (steveCommander.player.userSaveData.coin >= 200 && steveCommander.christmasTreeCount<3)
            {
                ParseChristmasTree(steveCommander);
                steveCommander.player.userSaveData.coin -= 200;
                steveCommander.christmasTreeCount++;
            }
            else
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易圣诞树失败，达到数量限制（3）或余额不足，需要200点数");
            }
            
        }else if (trim == "交易附魔槽位" || trim=="交易附魔槽位")
        {
            if (steveCommander.player.userSaveData.coin >= 120)
            {
                steveCommander.desireSpellCount++;
                steveCommander.SetMaxSpellCount();
                steveCommander.player.userSaveData.coin -= 120;
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易附魔槽位成功");
            }
            else
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易附魔槽位失败，需要120点数");
            }
        }else if (trim == "交易稀有武器")
        {
            if (steveCommander.player.userSaveData.coin >= 400)
            {
                steveCommander.giftWeaponCount++;
                steveCommander.player.userSaveData.coin -= 400;
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易稀有武器成功，发送兑换TNT/钓竿/召唤法杖/爆竹进行兑换");
            }
            else
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易稀有武器失败，需要400点数");
            }
        }else if (trim == "交易自爆羊" && PVEManager.Instance!=null)
        {
            if (steveCommander.player.userSaveData.coin >= 800)
            {
                (FightingManager.Instance.roundManager as McPveRoundManager)?.SelfExplosionSheep(steveCommander);
                steveCommander.player.userSaveData.coin -= 800;
            }
            else
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易自爆羊失败，需要800点数");
            }
        }else if (trim=="交易红包")
        {
            if (steveCommander.player.userSaveData.coin < 100)
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易红包失败，需要100点数");
            }else if (steveCommander.FindFirstValidSteve()==null)
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易红包失败，角色处于死亡状态");
            }
            else
            {
                var items = new List<string>()
                    { "获得25绿宝石", "获得100绿宝石","获得150绿宝石", "获得200绿宝石", "获得30礼物点数"};
                var randomList = new List<float>() { 40, 20, 20, 10,10};
                var sum = (int) randomList.Sum();

                if (randomList.Count != items.Count)
                {
                    TipsDialog.ShowDialog("红包概率配置错误，请检查", null);
                    Debug.LogError("红包概率配置错误，请检查");
                }

                for (int i = 0; i < randomList.Count; i++)
                {
                    randomList[i] = (randomList[i] / sum) * 100;
                    
                }

                var str = "";
                for (int i = 0; i < items.Count; i++)
                {
                    str += items[i] + "(" + randomList[i] + "%)";
                }

                Debug.Log("红包中奖概率如下：" + str);

                int rand = UnityEngine.Random.Range(0, 100);
                int currProb = 0;
                for (int i = 0; i < randomList.Count; i++)
                {
                    currProb += (int) randomList[i];
                    if (rand < currProb)
                    {
                        MessageBox._instance.AddMessage(steveCommander.player.userName + "抽奖结果为" + items[i]+"("+randomList[i]+"%)");
                        var ret = items[i];
                        string strTempContent = ret;
                        strTempContent = Regex.Replace(strTempContent, @"[^0-9]+", "");
                        var num = Int32.Parse(strTempContent);
                        if (ret.StartsWith("获得") && ret.EndsWith("绿宝石"))
                        {
                            
                                if (steveCommander.player.userSaveData != null)
                                {
                                    steveCommander.player.userSaveData.coin += num;
                                }
                            
                        }

                        if (ret.StartsWith("随机附魔"))
                        {
                            for (int j = 0; j < num; j++)
                            {
                                steveCommander.FindFirstValidSteve().RandomSpell(false, false);
                            }
                        }

                        if (ret.StartsWith("获得") && ret.EndsWith("礼物点数"))
                        {
                            if (steveCommander.player.userSaveData != null)
                            {
                                steveCommander.player.userSaveData.giftPoint += num;
                            }
                        }

                        break;
                    }
                }

                if (steveCommander.player.userSaveData != null) 
                    steveCommander.player.userSaveData.coin -= 100;
            }

        }
        else if (trim.StartsWith("交易托管者"))
        {
            bool IsNumeric(string str, out int result)
            {
                bool isNum;
                isNum = int.TryParse(str, out result);
                return isNum;
            }
            
            var index = 0;
            if (trim.Equals("交易托管者"))
            {
                index = MCPosManager.Instance.GetIndexByPos(steveCommander.ownerPlanet.GetVictimPosition());
            }
            else
            {
                var sub = trim.Substring(5);
                int num = 0;
                if (IsNumeric(sub,out num))
                {
                    index = num;
                }
                else
                {
                    ShowShopList();
                    return;
                }
                
            }
            
            if (steveCommander.player.userSaveData.coin >= 50)
            {
                steveCommander.SetLastGoPos(index);
                steveCommander.StartAutoBot();
                
                steveCommander.player.userSaveData.coin -= 50;
                MessageBox._instance.AddMessage("系统",steveCommander.player.userSaveData+"交易托管者成功");
            }
            else
            {
                MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"交易托管者失败，需要50绿宝石");
            }
            
        }
        else
        {
            ShowShopList();
        }
        
    }

    void ParseGiftWeapon(SteveCommander steveCommander, string trim)
    {
        var weaponName = trim.Substring(2);
        if (steveCommander.giftWeaponCount <= 0)
        {
            MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"兑换礼物武器次数不足，投喂这个好诶增加次数，次数不保留至下局，请在局内使用完");
            return;
        }
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
        {
            MessageBox._instance.AddMessage("系统", steveCommander.player.userName+"请玩家复活后再兑换礼物武器");
            return;
        }

        if (rareWeaponPrices.Find(x => x.weaponName == weaponName) == null)
        {
            MessageBox._instance.AddMessage("系统", steveCommander.player.userName+"兑换失败，请兑换礼物武器");

            return;
        }
        var success = validSteve.OnBuyWeaponSuccess(weaponName);
        if (success == false)
        {
            MessageBox._instance.AddMessage("系统", steveCommander.player.userName+"兑换失败，请检查兑换武器名称是否正确");
        }
        else
        {
            steveCommander.giftWeaponCount--;
        }
        
            
    }

    void ParseGatherSummons(SteveCommander steveCommander,string trim)
    {
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;

        var weapon = validSteve.GetActiveWeapon();
        if (weapon!=null && weapon.weaponName == "召唤法杖")
        {
            try
            {
                int logicPos = Int32.Parse(trim.Substring(2));
                if (logicPos > 100 || logicPos < 0)
                    return;

                var worldPos = fightingManager.mcPosManager.GetPosByIndex(logicPos);
                (weapon as SummonStaffWeapon)?.Gather(worldPos);
            }
            catch (Exception e)
            {
                Debug.Log("律令命令异常"+e.Message);
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
        var surrenderTime = steveCommander.ownerPlanet.planetCommanders.Count * 2;
        if (elapsedTime/60 < surrenderTime)
        {
            TipsDialog.ShowDialog("对局时间大于"+surrenderTime+"分钟后才能投降",null);
            
        }
        else
        {
            steveCommander.ParseSurrenderInMc();
        }
        
    }
    
    protected void ParseRandomSpell(SteveCommander steveCommander,bool rare,bool byGift)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();


        if (!validSteve)
        {
           steveCommander.toDoAfterRespawn.Add(() =>
           {
               ParseRandomSpell(steveCommander,rare,byGift);
               
           });
           return;
        }
            
        
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

    void ParseChristmasTree(SteveCommander steveCommander)
    {
        var validSteve = steveCommander.FindFirstValidSteve();


        if (!validSteve)
        {
            steveCommander.toDoAfterRespawn.Add(() =>
            {
                ParseChristmasTree(steveCommander);
               
            });
            return;
        }

        ResFactory.Instance.CreateChristmasTree(validSteve.transform.position);
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
    
    protected void ParseAddMaxHp(SteveCommander steveCommander,bool byGift)
    {
        if (byGift)
        {
            steveCommander.desireMaxHp ++ ;
        }
       
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;
        if(byGift)
            validSteve.AddMaxHp(8);
        else
        {
            if (steveCommander.point < 8)
            {
                steveCommander.commanderUi.LogTip("需要点数:8");
                return;
            }
            validSteve.AddMaxHp(8);
            steveCommander.AddPoint(-8);
        }
    }
    
    public bool ParseFixWeapon(SteveCommander steveCommander)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return false;


        if (steveCommander.point < 5)
        {
            steveCommander.commanderUi.LogTip("需要点数:5");
            return false;
        }

        validSteve.FixWeapon(25,true);
        steveCommander.AddPoint(-5);
        return true;

    }
    
    void ParseRareWeapon(SteveCommander steveCommander)
    {
        
        var validSteve = steveCommander.FindFirstValidSteve();
        if (!validSteve)
            return;
        
        validSteve.RandomRareWeapon();
    }
    
    protected void ParseAddPoint(SteveCommander steveCommander)
    {
        steveCommander.AddPoint(4f);
    }


    protected virtual void ParseGiftInMcMode(SteveCommander steveCommander,string giftName,int battery)
    {
        
    }
    
    /******************解析礼物*********************/
    protected override void ParseGift(int uid, string giftName,int battery)
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
        ParseGiftInMcMode(steveCommander,giftName,battery);

       

        if (giftName == "粉丝团灯牌")
        {
            //ParseChristmasTree(steveCommander);
        }

        if (giftName == "这个好诶")
        {
          
            steveCommander.giftWeaponCount++;
            MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"兑换礼物武器次数增加一次，输入兑换+武器名称兑换武器，如兑换钓竿，兑换TNT");
        }
        
    

        if (giftName == "辣条")
        {
            battery = 0;
        }

        if (steveCommander.flowerSpell == false && battery>0)
        {
            steveCommander.leftSpecificSpell++;
            MessageBox._instance.AddMessage("系统",steveCommander.player.userName+"通过电池礼物获得1次额外指定附魔次数（每局任意电池礼物可获得一次额外指定附魔次数，每局限一次）");
            steveCommander.flowerSpell = true;
            //steveCommander.desireSpellCount++;
            //steveCommander.SetMaxSpellCount();
        }
        
        EventCenter.Broadcast(EnumEventType.OnMcBatteryReceived,planet,battery==0? 0:battery/100);
    }

}
