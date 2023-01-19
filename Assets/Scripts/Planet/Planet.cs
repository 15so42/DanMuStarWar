using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using GameCode.Tools;
using Ludiq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineRenderPair
{
    public Planet planet;
    public LineRenderer line;

    public LineRenderPair(Planet planet, LineRenderer line)
    {
        this.planet = planet;
        this.line = line;
    }
}

public class ColonyPair
{
    public Planet owner;
    public float point;

    public ColonyPair(Planet owner, int point)
    {
        this.owner = owner;
        this.point = point;
    }
}


public class Planet : GameEntity,ITaskAble
{

    private FightingManager fightingManager;
    private Transform commanderGoContainer;
    [Header("指挥官配置")]
    public GameObject commanderPfb;

    public PlanetTerritory planetTerritory;
   
    //星球序号，用于宣战结盟等操作
    public int planetIndex = 0;
    [Header("Models")] 
    public List<PlanetConfig> planetConfigs;

    public string planetType;
    
    public List<PlanetCommander> planetCommanders=new List<PlanetCommander>();
    public List<GameObject> commanderGos=new List<GameObject>();
    public List<CommanderUI> commanderUis=new List<CommanderUI>();
    [HideInInspector]
    public PlanetResContainer planetResContainer;
    private TaskCenter[] taskCenters;

    
    public Player owner = null;
    
    public List<Player> enemyPlayers=new List<Player>();
    public List<Player> allyPlayers = new List<Player>();
    
    public List<Planet> enemyPlanets=new List<Planet>();
    public List<Planet> colonyPlanets = new List<Planet>();
    public List<Planet> allyPlanets = new List<Planet>();

    [Header("PlanetUI")] public PlanetUI planetUi;

    public Color planetColor;
    [Header("驻守模块")]
    //ringUI
    private ColonyRingUi ringUi;
    public float needRingPoint = 100;
    
    public Action<float, float> onColonyPointChanged;
    public Vector3 ringOffset;
    public Vector3 ringUiScale;
    //占领点计算
    public List<ColonyPair> colonyPairs=new List<ColonyPair>();
    [Header("被占领点数")] public float colonyPoint;
    public bool occupied = false;
    
    public Transform spawnPoint;
    
    //LineRenders
    public List<LineRenderPair> enemyPlanetLines=new List<LineRenderPair>();
    public List<LineRenderPair> colonyPlanetLines=new List<LineRenderPair>();
    private List<LineRenderer> lineRenderers=new List<LineRenderer>();
    
    //自己单位管理
    public List<BattleUnit> battleUnits=new List<BattleUnit>();

    
    public int maxSkillCount = 3;
    //自动抽卡，每隔三秒，卡槽没满就抽
    public bool autoRoll = true;
    private float autoRollTimer = 0;
    
    //自动造兵，每隔三秒，发送一次命令
    public bool autoCreateUnit = false;
    private float autoCUTimer = 0;
    public string unitRateStr = "132";//132表示概率
    
    //给指挥官分点数：
    public float commanderPointTimer = 0;
    public float commanderPointCd = 5;
    
    //紧急维修
    public bool urgentRepair = false;

    private UnityTimer.Timer refreshIronGolem;
    public void RefreshIronGolem()
    {
        refreshIronGolem?.Cancel();
        refreshIronGolem = UnityTimer.Timer.Register(480, () =>
        {
            AddTask(new PlanetTask(new TaskParams(TaskType.Create, "BattleUnit_IronGolem", 5), null));
        });
    }
      
    void Awake()
    {
        autoRoll = true;
        
        base.Awake();
        
        
        taskCenters = GetComponents<TaskCenter>();
        
        planetResContainer = GetComponent<PlanetResContainer>();
        
       
        
      
        
        //事件绑定
        //任意玩家加入游戏均设置为自己的敌人，除非后期主动结盟
        EventCenter.AddListener<Player>(EnumEventType.OnPlayerJoined,OnPlayerJoined);
        
        
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetCreated,OnPlanetCreated);
        
        EventCenter.AddListener<BattleUnit>(EnumEventType.OnBattleUnitCreated,OnBattleUnitCreated);
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetDie,DestroyWarLine);
        
        
        //驻守
        EventCenter.AddListener<Planet,Planet>(EnumEventType.OnPlanetOccupied,OnPlanetOccupied);
        EventCenter.AddListener<Planet,Planet>(EnumEventType.OnColonyLost,OnColonyLost);
        
        
       
    }

    public void UrgentRepair(int uid)
    {
        if (!urgentRepair)
        {
            var planetCommander = GetCommanderByUid(uid);
            SkillManager.Instance.AddSkill("Skill_紧急维修_LV1",this,planetCommander);
            urgentRepair = true;
        }
        else
        {
            LogTip("紧急维修次数已用尽");
        }
            
    }

    public void SetRingPoint(float point)
    {
        colonyPoint = point;
        colonyPoint = Mathf.Clamp(colonyPoint, 0, needRingPoint);
        
        onColonyPointChanged?.Invoke(colonyPoint,needRingPoint);
    }


    public void OnPlanetOccupied(Planet attacker,Planet colony)
    {
        if (attacker == this && colonyPlanets.Contains(colony)==false)
        {
            LogTip("占领" + colony.planetIndex);
            colony.occupied = true;
            if(colony.hpUI)
                colony.hpUI.SetColor(attacker.planetColor);
            colonyPlanets.Add(colony);
            var line = LineRenderManager.Instance.SetLineRender(transform.position, colony.transform.position,
                LineRenderManager.Instance.colonyLinePfb);
            colonyPlanetLines.Add(new LineRenderPair(colony, line));
        }
    }
    
    public void OnColonyLost(Planet owner,Planet colony)
    {
        if (owner==this && colonyPlanets.Contains(colony))
        {
            LogTip("星球"+colony.planetIndex+"失守");
            colony.occupied = false;
            colonyPlanets.Remove(colony);
            DestroyDefendLine(colony);
            
        }
    }

    public void Gather(int uid,Planet targetPlanet)
    {
        var planetCommander = GetCommanderByUid(uid);
        var gatherUi=GameManager.Instance.uiManager.CreateGatherUi(targetPlanet,this,planetCommander);
    }


    /// <summary>
    /// uid表示指挥官id
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="planet"></param>
    public void Recall(int uid,Planet planet)
    {
        var planetCommander = GetCommanderByUid(uid);
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] && battleUnits[i].isDefending && battleUnits[i].defendingPlanet == planet && battleUnits[i].planetCommander==planetCommander)
            {
                battleUnits[i].isDefending = false;
                battleUnits[i].defendingPlanet = null;
                battleUnits[i].Recall();
            }
        }
    }

    public void RecallAll(int uid)
    {
        var planetCommander = GetCommanderByUid(uid);
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] && battleUnits[i].isDefending && battleUnits[i].planetCommander==planetCommander)
            {
                battleUnits[i].isDefending = false;
                battleUnits[i].defendingPlanet = null;
                battleUnits[i].Recall();
            }
        }
    }
    
    /// <summary>
    /// 被驻守,表示被colonist驻守
    /// </summary>
    /// <param name="colonist"></param>
    /// <param name="point"></param>
    public void Defend(Planet colonist,int uid,float point)
    {
        if(colonist==this)
            return;
        
        if(owner!=null && owner.die==false){//目标星球有玩家并且存活，无法占领
            colonist.Recall(uid,this);//殖民者召回在本星球上的战舰
            SetRingPoint(0);
            return;
        }
            
        var pair = colonyPairs.Find(x => x.owner == colonist);
        if ( pair == null)
        {
            pair = new ColonyPair(colonist, 0);
            colonyPairs.Add(pair);
        }

        var removedOther = false;
        for (int i = 0; i < colonyPairs.Count; i++)
        {
            var p = colonyPairs[i];
            if (p != pair)
            {
                if (p.point > 0)
                {
                    p.point -= point;//削除其余势力的点数
                    if (p.point <= 0)//其余势力被削弱到0
                    {
                        EventCenter.Broadcast(EnumEventType.OnColonyLost,p.owner,this);
                    }
                    removedOther = true;
                }
                else//如果这个人的势力已经被削减，去削减另一个人的势力
                {
                    continue;
                }
                
                break;
            }
        }

        if (removedOther == false && colonyPoint<needRingPoint)
        {
            pair.point += point;
            ringUi.SetColor(pair.owner.planetColor);
        }
            

        var sumPoint = 0f;
        //统计所有势力的点数
        for (int i = 0; i < colonyPairs.Count; i++)
        {
            var p = colonyPairs[i];
           
                sumPoint += p.point;//削除其余势力的点数
                if (sumPoint >= needRingPoint)
                {
                    EventCenter.Broadcast(EnumEventType.OnPlanetOccupied,p.owner,this);
                    break;
                }
            
        }
        SetRingPoint(sumPoint);

    }
    

    void OnBattleUnitCreated(BattleUnit battleUnit)
    {
        if (battleUnit.ownerPlanet == this)
        {
            battleUnits.Add(battleUnit);
        }
    }


    public void Start()
    {
        base.Start();
        hpUI.SetColor(planetColor);
        OnHpChanged(props.hp,props.maxHp,props.shield,props.maxShield);
        fightingManager = GameManager.Instance.fightingManager;
        
        ringUi = GameManager.Instance.uiManager.CreateRingUi(this);
        ringUi.Init(this,planetColor);
        SetRingPoint(0);
        //if(colonyPoint==0)
        //    ringUi.gameObject.SetActive(false);

        if (fightingManager.gameMode == GameMode.MCWar || fightingManager.gameMode==GameMode.Marble)
        {
            props.maxHp = 450;
            props.hp = 450;
        }
            

        
        foreach (var t in taskCenters)
        {
            t.Init(this);
        }
        
        PlanetConfig planetConfig = null;
        //PlanetConfigs
        planetConfigs = GetComponentsInChildren<PlanetConfig>().ToList();
        foreach (var p in planetConfigs)
        {
            if(p.gameObject.name!=planetType){
                p.gameObject.SetActive(false);
                
            }
            else
            {   
                
                planetConfig = p;
                continue;
                
            }
        }//先隐藏所有的模型，由SetUp决定使用哪种星球后再显示
        if (planetConfig!=null)
        {
            if (planetConfig.spawnCloud == false)
            {
                GetComponent<CloudSpawner>().Close();
            }

           

            planetResContainer.allRes = planetConfig.allRes;

        }
        
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_纸飞机",5)));
        if (planetResContainer.GetResNumByType(ResourceType.Population) > 0)
        {
            
        }
        
        
        EventCenter.Broadcast(EnumEventType.OnPlanetCreated,this);
        planetUi = GameManager.Instance.uiManager.CreatePlanetUI(this);
        planetUi.Init(this,fightingManager.gameMode);

        if (fightingManager.gameMode == GameMode.MCWar || fightingManager.gameMode==GameMode.Marble)
        {
            transform.localScale=Vector3.one*0.5f;
            planetUi.gameObject.SetActive(false);
        }

        if (fightingManager.gameMode == GameMode.Marble)
        {
            //planetUi.gameObject.SetActive(true);
            autoRoll = false;
            SkillManager.Instance.AddSkill("Skill_永久腐蚀_LV1",this,null);
        }

        planetTerritory = GetComponent<PlanetTerritory>();

        foreach (var p in enemyPlanets)
        {
            //var lr=LineRenderManager.Instance.SetLineRender(transform.position, p.transform.position);
            //lineRenderers.Add(lr);
        }
    }

    /// <summary>
    /// 宣战
    /// </summary>
    /// <param name="planet"></param>
    public void ClaimWar(int commanderUid,Planet planet)
    {
        if (planet == this)
        {
            TipsDialog.ShowDialog("不能对自己宣战",null);
            return;
        }
        if (this.die || planet.die)
        {
            TipsDialog.ShowDialog("无法对已淘汰星球宣战",null);
            return;
        }
        if (owner==null || planet.owner==null)
        {
            TipsDialog.ShowDialog("无法对无人星球宣战",null);
            return;
        }

        if (enemyPlanets.Contains(planet) == false)
        {
            enemyPlanets.Add(planet);
            enemyPlayers.Add(planet.owner);
        }
        

        var enemyUnits = planet.battleUnits;
        
        //派遣军队
        for (int i = 0; i < battleUnits.Count; i++)
        {
            var chance = Random.Range(0, 10) < 10;//全部派出
            if (chance && battleUnits[i] && !battleUnits[i].die && battleUnits[i].canAttack && battleUnits[i].planetCommander.uid==commanderUid)
            {
                GameEntity target = planet;
                if (enemyUnits.Count > 0)
                {
                    //target = enemyUnits[Random.Range(0, enemyUnits.Count)];
                }
                
                battleUnits[i].ClaimWar();
                battleUnits[i].SetChaseTarget(target);
            }
        }
        
        
        if (enemyPlanetLines.Find(x => x.planet == planet) == null)
        {
            var line = LineRenderManager.Instance.SetLineRender(transform.position, planet.transform.position);
            enemyPlanetLines.Add(new LineRenderPair(planet, line));
        }
        
    }

   
    public void ClaimDefend(int uid,Planet planet)
    {
        if(planet==null)
            return;
        if (planet.owner != null && planet.owner.die==false)
        {
            LogTip("目标星球已被占领");
            return;
        }
        if (planet == this)
        {
            LogTip("不能驻守自己");
            return;
        }

        enemyPlanets.Remove(planet);
        DestroyWarLine(planet);
        
        var line = LineRenderManager.Instance.SetLineRender(transform.position, planet.transform.position,LineRenderManager.Instance.colonyLinePfb);
        Destroy(line.gameObject,5f);

        var planetCommander = GetCommanderByUid(uid);
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i].canDefendOtherPlanet==false || battleUnits[i].isDefending || battleUnits[i].planetCommander!=planetCommander)
            {
                continue;
            }

            battleUnits[i].SetDefendTarget(planet);
            
        }
       
    }

   

    void DestroyWarLine(Planet planet)
    {
        if (planet == this)
        {
            //自己死了，把自己连别人的线删了
            foreach (var t in enemyPlanetLines)
            {
                if(t==null || t.line==null || t.line.gameObject==null){continue;}
                Destroy(t.line.gameObject);
            }
            
            foreach (var p in colonyPlanetLines)
            {
                if(p==null || p.line==null || p.line.gameObject==null){continue;}
                {
                    Destroy(p.line.gameObject);
                }
            }
            
            return;
        }
        //别人死了，把自己连别人的线删了
        var line = enemyPlanetLines.Find(x => x.planet == planet)?.line;
        if(line)
            Destroy(line.gameObject);
       
    }
    
    void DestroyDefendLine(Planet planet)
    {
        
        var colonyLinePair = colonyPlanetLines.Find(x => x.planet == planet);
        if (colonyLinePair!=null && colonyLinePair.line!=null )
        {
            colonyPlanetLines.Remove(colonyLinePair);
            Destroy(colonyLinePair.line.gameObject);
        }
            
    }

    public void SetIndex(int index)
    {
        planetIndex = index;
        planetUi.SetIndex(planetIndex);
        gameObject.name = planetIndex+"";
    }
    void OnPlanetCreated(Planet planet)
    {
        //if(this!=planet)
            //enemyPlanets.Add(planet);
    }


    void OnPlayerJoined(Player newPlayer)
    {
        if (newPlayer!=null && owner!=null && newPlayer != owner)
        {
            enemyPlayers.Add(newPlayer);//新加入的玩家被当作敌人
        }
        
    }

    public void SetUpPlanet(string targetPlanetType,Color color)//在Awake前执行
    {
        planetType = targetPlanetType;
        gameObject.SetActive(true);
        planetColor = color;
        
        
    }

    //由fighingManager在玩家进入游戏时选择星球并占领
    public void SetOwner(Player player,PlanetCommander planetCommander)
    {
        this.owner = player;
        planetUi.SetOwner(player,fightingManager.gameMode);
        if (planetCommanders.Find(x => x.uid == player.uid) == null && planetCommander==null)
        {
            //planetCommanders.Add();
            if (fightingManager.gameMode == GameMode.MCWar|| fightingManager.gameMode==GameMode.Marble)
            {
                var commander = new SteveCommander(player.uid, player, planetColor);
                AddCommander(commander,1);
                
            }
            else
            {
                AddCommander(new PlanetCommander(player.uid,player,planetColor),1);
            }
            
        }
        else
        {
            //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_IronGolem",1),null));
            AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_McPlanetTower",1),null));
            return;
        }
        
        
        ringUi.gameObject.SetActive(false);
        
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_探索船",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));

        gameObject.name = player.userName;
        
        //autoCreateUnit = true;
        
        //添加技能测试
        if (FightingManager.Instance.gameMode == GameMode.Normal)
        {
            var initSkills = SkillManager.Instance.initSkill;
            SkillManager.Instance.AddSkill(initSkills[Random.Range(0,initSkills.Count)].skillName, this,planetCommanders[0]);
        }

      
        
    }

    public void AddCommander(PlanetCommander planetCommander,int uiArea)
    {
        if (planetCommanders.Count == 0)
        {
            commanderGoContainer=new GameObject(planetCommander.player.userName).transform;
            commanderGoContainer.transform.position = transform.position-Vector3.forward*65;
            commanderGoContainer.AddComponent<RotateSelf>().rotateDir=Vector3.up;
        }
        
        planetCommander.Init(this);
        planetCommanders.Add(planetCommander);
        var mark=GameObject.Instantiate(commanderPfb);
        Vector3 worldPos = transform.position;
        var curCommanderLength = planetCommanders.Count;
        var transform1 = commanderGoContainer.transform;
        if (curCommanderLength < 7)
        {
            //Debug.Log(gameObject.name+ curCommanderLength);
            worldPos = transform1.position+ transform1.right * (Mathf.Sin(  45+Mathf.Deg2Rad*(curCommanderLength)*360/8) * 8) + transform1.forward * (Mathf.Cos(45+Mathf.Deg2Rad*(curCommanderLength)*360/8) * 8);
        }
        else
        {
            worldPos = transform1.position+ transform1.right * (Mathf.Sin(  45+Mathf.Deg2Rad*(curCommanderLength)*360/8) * 14) + transform1.forward * (Mathf.Cos(45+Mathf.Deg2Rad*(curCommanderLength)*360/8) * 14);
        }

        mark.transform.position = worldPos;
        mark.gameObject.SetActive(false);
        mark.transform.SetParent(commanderGoContainer);
        CommanderUI commanderUi = null;
        if (fightingManager.gameMode == GameMode.MCWar || fightingManager.gameMode==GameMode.Marble)
        {
            commanderUi=GameManager.Instance.uiManager.CreateSteveCommanderUi(mark,uiArea);
        }
        else
        {
            commanderUi=GameManager.Instance.uiManager.CreateCommanderUi(mark,uiArea);
        }
        
        commanderUi.Init(mark,planetCommander);
        if (fightingManager.gameMode == GameMode.MCWar|| fightingManager.gameMode==GameMode.Marble)
        {
            planetCommander.color = planetColor;
        }
        commanderUi.SetColor(planetCommander.color);
        planetCommander.commanderUi = commanderUi;
        planetCommander.UpdateLastMsgTime(Time.time);
        commanderGos.Add(mark);
        commanderUis.Add(commanderUi);

        if (fightingManager.gameMode == GameMode.BattleGround)
        {
            AddTask(new PlanetTask(new TaskParams(TaskType.Create,GameConst.BattleUnit_WARPLANE,1),planetCommander));
            //AddTask(new PlanetTask(new TaskParams(TaskType.Create,GameConst.BattleUnit_LONGBOW,1),planetCommander));
            //AddTask(new PlanetTask(new TaskParams(TaskType.Create,GameConst.BattleUnit_GUARDPLANE,1),planetCommander));
            //AddTask(new PlanetTask(new TaskParams(TaskType.Create,GameConst.BattleUnit_PACMAN,1),planetCommander));
            
            
        }

        if (fightingManager.gameMode == GameMode.MCWar || fightingManager.gameMode==GameMode.Marble)
        {
            (planetCommander as SteveCommander).CreateSteve();
        }
            
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,radius);
    }

    public Player GetOwner()
    {
        return owner;
    }

    public void AddTask(PlanetTask planetTask)
    {
        taskCenters[0].AddTask(planetTask);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(die || !gameObject)
            return;
        foreach (var taskCenter in taskCenters)
        {
            taskCenter.Run();
        }

        autoRollTimer += Time.deltaTime;
        if (autoRollTimer > 6 && autoRoll && owner!=null)
        {
            if (skillContainer.skills.Count < maxSkillCount)
            {
                RollSkill(planetCommanders[0].uid);
                autoRollTimer = 0;
            }
        }

        ///分配玩家点数
        commanderPointTimer += Time.deltaTime;
        if (true)
        {
            if (commanderPointTimer > commanderPointCd)
            {
                var playerCount = fightingManager.players.Count;
                float teamPoint = playerCount / (fightingManager.gameMode==GameMode.Normal?playerCount:2f);//混战模式每次拿一点
                int hangUpPlayerCount = 0;
                var totalPoint = teamPoint;
                //点数增加
                totalPoint *= 2;
            
                //挂机处理
                for (int i = 0; i < planetCommanders.Count; i++)
                {
                    var planetCommander = planetCommanders[i];
                    planetCommander.HangUpCheck();
                    planetCommander.Update();
                    if (planetCommander.hangUp)
                    {
                        totalPoint += planetCommander.point*1.1f;
                        planetCommander.AddPoint(planetCommander.point*-1);//清空挂机者点数
                        hangUpPlayerCount++;
                    }
                    
                }

                var commanderCount = planetCommanders.Count;

                //var point = totalPoint / (commanderCount - hangUpPlayerCount);
                var point = totalPoint / commanderCount ;
                for (int i = 0; i < planetCommanders.Count; i++)
                {
                    var commander = planetCommanders[i];
                    if(commander.hangUp==false)
                        commander.AddPoint(point);
                }

                commanderPointTimer = 0;
            }
        }

        autoCUTimer += Time.deltaTime;
        if (autoCUTimer > 6 && autoCreateUnit)//废弃的星球大战AI模块
        {
            int sum = 0;
            
            for (int i = 0; i < unitRateStr.Length; i++)
            {
                int s = unitRateStr[i];
                sum+=s;
            }

            if (sum == 0)
            {
                //DONothing
            }
            else
            {
                for (int i = 0; i < unitRateStr.Length; i++)
                {
                    int s = unitRateStr[i];
                    int random = Random.Range(0, sum);
                    if (random < s)
                    {
                        var str = "m" + (i + 1);
                        SendDanMu(str);
                        LogTip(str);
                    }
                }
            }

            autoCUTimer = 0;

        }
    }

   

    public void SendDanMu(string s)
    {
        EventCenter.Broadcast(EnumEventType.OnDanMuReceived,owner.userName,owner.uid,"0",s);
    }

    public override void LogTip(string tip)
    {
        //Debug.Log(tip);
        if(die || !planetUi)
            return;
        
        planetUi.LogTip(tip);
    }


    public PlanetCommander GetCommanderByUid(int uid)
    {
        return planetCommanders.Find(x => x.uid == uid);
    }
   
    
    public int GetTechLevelByRes()
    {
        
        
        int techLevel = 1;
        var techPoint= planetResContainer.GetResNumByType(ResourceType.Tech);

        if (fightingManager.gameMode == GameMode.Normal)
        {
            if (techPoint > 200)
            {
                techLevel = 2;
            }
        
            if (techPoint > 800)
                techLevel = 3;
        
            if (techPoint > 3000)
                techLevel = 4;
            return techLevel;
        }
        else
        {
            if (techPoint > 150)
            {
                techLevel = 2;
            }
        
            if (techPoint > 400)
                techLevel = 3;
        
            if (techPoint > 800)
                techLevel = 4;
            return techLevel;
        }
      
    }

    #region MyRegion

    public SkillBase GetSkillByIndex(int index)//使用的从1开始的读法
    {
        if (index>0 && skillContainer.skills.Count >= index && skillContainer.skills[index-1])
        {
            return skillContainer.skills[index-1];
        }

        return null;
    }

    public void RollSkillBG(int commanderUid,bool byGift)
    {
        var commander = GetCommanderByUid(commanderUid);
        if (commander!=null)
        {
            if (commander.point < 3 && !byGift)
            {
                commander.commanderUi.LogTip("需要点数:3");
                return;
            }
            
            if (skillContainer.skills.Count >= maxSkillCount)
            {
                LogTip("技能栏位已满");
                return;
            }
            
            skillContainer.AddRandomSkill(GetTechLevelByRes(),commander);
            if(!byGift)
                commander.AddPoint(-3);
        }
    }

    public void RollSkill(int commanderUid)
    {
        if (planetResContainer.GetResNumByType(ResourceType.DicePoint) <= 0)
        {
            LogTip("骰子点数不足");
            return;
        }
        if (skillContainer.skills.Count >= maxSkillCount)
        {
            LogTip("技能栏位已满");
            return;
        }

        var commander = GetCommanderByUid(commanderUid);
        skillContainer.AddRandomSkill(GetTechLevelByRes(),commander);
        planetResContainer.ReduceRes(ResourceType.DicePoint,1);
    }

    public void UseSkillBG(int commanderUid,int index)
    {
        var skill = GetSkillByIndex(index);
        if (skill == null)
        {
            LogTip("序号错误");
            return;
        }
        var commander = GetCommanderByUid(commanderUid);
        
        if (commander.point < skill.usePoint)
        {
            commander.commanderUi.LogTip("需要点数:"+skill.usePoint);
            return;
        }

        
        var useSuccess=skillContainer.UseSkill(index-1,commander);
        if(useSuccess)
            commander.AddPoint(-1*skill.usePoint);
    }
    
    public void UseSkill(int commanderUid,int index)
    {
        var skill = GetSkillByIndex(index);
        if (skill == null)
        {
            LogTip("序号错误");
            return;
        }
        
        if (planetResContainer.GetResNumByType(ResourceType.DicePoint) < skill.usePoint)
        {
            LogTip("骰子点数不足");
            return;
        }

        var commander = GetCommanderByUid(commanderUid);
        var useSuccess=skillContainer.UseSkill(index-1,commander);
        if(useSuccess)
            planetResContainer.ReduceRes(ResourceType.DicePoint,skill.usePoint);
        
            
    }

    public void ChangeSkillBG(int commanderUid, int index)
    {
        var commander = GetCommanderByUid(commanderUid);
        
        
        if (commander!=null)
        {
            var skill = GetSkillByIndex(index);
            if (skill == null)
            {
                commander.commanderUi.LogTip("序号错误");
                return;
            }
            
            if (commander.point < skill.removePoint+1)
            {
                commander.commanderUi.LogTip("需要点数:"+skill.removePoint+1);
                return;
            }
            skillContainer.ChangeSkill(index-1,commander);
            commander.AddPoint(-1*(skill.removePoint+1));
        }
    }
    
    public void ChangeSkill(int commanderUid,int index)
    {
        
        var skill = GetSkillByIndex(index);
        if (skill == null)
        {
            LogTip("序号错误");
            return;
        }
        
        if (planetResContainer.GetResNumByType(ResourceType.DicePoint) < skill.removePoint+1)
        {
            LogTip("换技能骰子点数不足");
            return;
        }
        var commander = GetCommanderByUid(commanderUid);
        skillContainer.ChangeSkill(index-1,commander); 
        planetResContainer.ReduceRes(ResourceType.DicePoint,skill.removePoint+1);
        
        
    }

    public void BuySkillBG(int commanderUid, int index)
    {
        
        var commander = GetCommanderByUid(commanderUid);
        if (commander.point > 1)
        {
            var buySuccess=skillContainer.BuySkill(index-1,fightingManager.gameMode,commander);
            if (buySuccess)
            {
                commander.AddPoint(-1);
            }
        }
        
    }
    
    public void BuySkill(int commanderUid,int index)
    {
        if (planetResContainer.GetResNumByType(ResourceType.DicePoint) < 1)
        {
            LogTip("买技能需要1个骰子");
            return;
        }
        if (skillContainer.skills.Count >= maxSkillCount+1)
        {
            LogTip("技能栏位已满");
            return;
        }
        var commander = GetCommanderByUid(commanderUid);
        var buySuccess=skillContainer.BuySkill(index-1,fightingManager.gameMode,commander); 
        if(buySuccess)
            planetResContainer.ReduceRes(ResourceType.DicePoint,1);
        
    }
    
    public void RemoveSkill(int uid,int index)
    {
        var skill = GetSkillByIndex(index);
        if (skill == null)
        {
            LogTip("序号错误");
            return;
        }
        
        if (planetResContainer.GetResNumByType(ResourceType.DicePoint) < skill.removePoint)
        {
            LogTip("移除技能所需点数不够");
            return;
        }

        skillContainer.RemoveSkill(index-1);
        planetResContainer.ReduceRes(ResourceType.DicePoint,skill.removePoint);
    }
    #endregion

    public override void Die()
    {
        base.Die();
        
        refreshIronGolem?.Cancel();
        // try
        // {
        EventCenter.Broadcast(EnumEventType.OnPlanetDie, this);
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError("死亡事件异常："+e);
        // }
        
        
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i])
            {
                battleUnits[i].Die();
            }
            
        }

        planetCommanders.Clear();
        for (int i = 0; i < commanderGos.Count; i++)
        {
            Destroy(commanderGos[i]);
           
        }

        for (int i = 0; i < commanderUis.Count; i++)
        {
            Destroy(commanderUis[i].gameObject);
        }


        
       

        if (lastAttacker != null && fightingManager.gameMode==GameMode.Normal)//死亡时把一般资源给击杀者
        {

            var attackerOwnerEntity = lastAttacker.GetAttackerOwner();
            
            
            if (attackerOwnerEntity && attackerOwnerEntity as Planet)
            {
                SkillManager.Instance.AddSkill("Skill_紧急维修_LV1",attackerOwnerEntity,(attackerOwnerEntity as Planet).planetCommanders[0]);
                var attackerOwnerPlanet = attackerOwnerEntity as Planet;
                var tech=planetResContainer.GetResNumByType(ResourceType.Tech);
                var dice = planetResContainer.GetResNumByType(ResourceType.DicePoint);
                
                planetResContainer.ReduceRes(ResourceType.Tech,tech/2);
                planetResContainer.ReduceRes(ResourceType.DicePoint,dice/2);

                if (attackerOwnerPlanet != null)
                {
                    attackerOwnerPlanet.planetResContainer.AddRes(ResourceType.Tech, tech / 2);
                    attackerOwnerPlanet.planetResContainer.AddRes(ResourceType.DicePoint, dice / 2);
                }
            }
        }
        ringUi.gameObject.SetActive(true);
        ringUi.UpdateRing(0,100);
        //Destroy(ringUi);
        Destroy(hpUI.gameObject);
        //hpUI.gameObject.SetActive(false);
        //Destroy(planetUi.gameObject);
        planetUi.UpdateOwnerOnDie();
        //gameObject.SetActive(false);
        //owner = null;
        if(owner!=null)//野生星球没有主人
            owner.die = true;

    }

    public void ShowCommanderPosition(int uid)
    {
        var commander = GetCommanderByUid(uid);
        if (commander != null)
        {
            for (int i = 0; i < battleUnits.Count; i++)
            {
                if (battleUnits[i] != null && battleUnits[i].die == false &&
                    battleUnits[i].planetCommander == commander)
                {
                    battleUnits[i].LogTip(commander.player.userName);
                }
            }
        }
    }
    
    public void GoWhere(int uid,int index,bool escape)
    {
        var commander = GetCommanderByUid(uid);
        if (commander != null)
        {
            
            
            if (index < 0 || index >= fightingManager.mcPosManager.positions.Count)
            {
                commander.commanderUi.LogTip("序号错误");
                return;
            }
            var pos = fightingManager.mcPosManager.GetPosByIndex(index);
            for (int i = 0; i < battleUnits.Count; i++)
            {
                if (battleUnits[i] != null && battleUnits[i].die == false &&
                    battleUnits[i].planetCommander == commander && battleUnits[i].GetType()==typeof(Steve))
                {
                    (battleUnits[i] as McUnit).SetGuardStats(false);
                    battleUnits[i].GoMCWorldPos(pos,escape);
                    
                    
                    //(battleUnits[i] as McUnit).isGuard = false;
                    

                    //var targetPos = fightingManager.mcPosManager.GetPosByIndex(targetIndex);
                    //battleUnits[i].LogTip(commander.player.userName);
                }
            }
            (commander as SteveCommander)?.SetLastGoPos(index);
        }
    }

    public void Guard(int uid)
    {
        var commander = GetCommanderByUid(uid);

        var steve = (commander as SteveCommander).FindFirstValidSteve();
        if (steve)
        {
            steve.SetGuardStats(true);
            steve.guardPos = steve.transform.position;
        }
        // if (commander != null)
        // {
        //     for (int i = 0; i < battleUnits.Count; i++)
        //     {
        //         if (battleUnits[i] != null && battleUnits[i].die == false &&
        //             battleUnits[i].planetCommander == commander)
        //         {
        //             
        //             (battleUnits[i] as McUnit).SetGuardStats(true);
        //             
        //             (battleUnits[i] as McUnit).guardPos = battleUnits[i].transform.position;
        //
        //         }
        //     }
        // }
    }
    
    

    public override GameEntity GetAttackerOwner()
    {
        return this;
    }

    public override GameEntity GetVictimOwner()
    {
        return this;
    }

    public override void OnStartWaitingJoin()
    {
        Destroy(planetUi.gameObject);
        Destroy(ringUi.gameObject);
        
        base.OnStartWaitingJoin();
    }

    public override BattleUnitProps.HpAndShield OnAttacked(AttackInfo attackInfo)
    {
        
        var newAttackInfo=new AttackInfo(attackInfo.attacker,attackInfo.attackType,attackInfo.value);
        if (newAttackInfo.value > 45)
        {
            newAttackInfo.value = 45;
        }
        var hpAndShield = base.OnAttacked(newAttackInfo);

        if (Math.Abs(supportDistance) < 0.5f)
            return hpAndShield;
        if (attackInfo.attacker == null || attackInfo.attackType == AttackType.Heal ||
            attackInfo.attacker.GetAttackerOwner() == GetVictimOwner()) //同一阵营
        {
            return hpAndShield;
        }

        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] != null && Vector3.Distance(battleUnits[i].transform.position, transform.position) <
                supportDistance)
            {
                var supportAble = battleUnits[i].GetComponent<ISupportAble>();
                supportAble?.Support(attackInfo.attacker as BattleUnit);
            }
        }

        return hpAndShield;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
