using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
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


public class Planet : GameEntity
{

    //星球序号，用于宣战结盟等操作
    public int planetIndex = 0;
    [Header("Models")] 
    public List<PlanetConfig> planetConfigs;

    public string planetType;
    
    private PlanetCommander planetCommander;
    [HideInInspector]
    public PlanetResContainer planetResContainer;
    private TaskCenter[] taskCenters;

    [Header("可否被占领")]
    public bool canBeOwner = false;
    public Player owner = null;
    
    public List<Player> enemyPlayers=new List<Player>();
    public List<Player> allyPlayers = new List<Player>();
    
    public List<Planet> enemyPlanets=new List<Planet>();
    public List<Planet> colonyPlanets = new List<Planet>();

    [Header("PlanetUI")] public PlanetUI planetUi;

    public Color planetColor;
    //ringUI
    private ColonyRingUi ringUi;
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

      
    void Awake()
    {
        base.Awake();
        
        planetCommander = GetComponent<PlanetCommander>();
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

    public void SetRingPoint(float point)
    {
        colonyPoint = point;
        colonyPoint = Mathf.Clamp(colonyPoint, 0, 100);
        
        onColonyPointChanged?.Invoke(colonyPoint,100);
    }


    public void OnPlanetOccupied(Planet attacker,Planet colony)
    {
        if (attacker == this && colonyPlanets.Contains(colony)==false)
        {
            LogTip("占领" + colony.planetIndex);
            occupied = true;
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
            occupied = false;
            DestroyDefendLine(colony);
            colonyPlanets.Remove(colony);
        }
    }



    public void Recall(Planet planet)
    {
        for (int i = 0; i < battleUnits.Count; i++)
        {
            if (battleUnits[i] && battleUnits[i].isDefending && battleUnits[i].defendingPlanet == planet)
            {
                battleUnits[i].isDefending = false;
                battleUnits[i].defendingPlanet = null;
                battleUnits[i].Recall();
            }
        }
    }
    
    /// <summary>
    /// 被驻守
    /// </summary>
    /// <param name="colonist"></param>
    /// <param name="point"></param>
    public void Defend(Planet colonist,float point)
    {
        
        if(owner!=null)
            return;
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
                
                break;
            }
        }

        if (removedOther == false && colonyPoint<100)
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
                if (sumPoint >= 100)
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
        
        
        ringUi = GameManager.Instance.uiManager.CreateRingUi(this);
        ringUi.Init(this,planetColor);
        SetRingPoint(0);
        //if(colonyPoint==0)
        //    ringUi.gameObject.SetActive(false);
        

        
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

            if (planetConfig.canBeOwner == false)
            {
                canBeOwner = false;
            }

            planetResContainer.allRes = planetConfig.allRes;

        }
        
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_纸飞机",5)));
        if (planetResContainer.GetResNumByType(ResourceType.Population) > 0)
        {
            
        }
        
        
        EventCenter.Broadcast(EnumEventType.OnPlanetCreated,this);
        planetUi = GameManager.Instance.uiManager.CreatePlanetUI(this);
        planetUi.Init(this);
        
        //添加技能测试
        var initSkills = SkillManager.Instance.initSkill;
        SkillManager.Instance.AddSkill(initSkills[Random.Range(0,initSkills.Count)].skillName, this);

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
    public void ClaimWar(Planet planet)
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
        
        enemyPlanets.Add(planet);
        enemyPlayers.Add(planet.owner);

        var enemyUnits = planet.battleUnits;
        
        //派遣军队
        for (int i = 0; i < battleUnits.Count; i++)
        {
            var chance = Random.Range(0, 10) < 5;
            if (chance && battleUnits[i] && !battleUnits[i].die && battleUnits[i].canAttack)
            {
                GameEntity target = planet;
                if (enemyUnits.Count > 0)
                {
                    target = enemyUnits[Random.Range(0, enemyUnits.Count)];
                }
                battleUnits[i].SetChaseTarget(target);
            }
        }
        
        var line = LineRenderManager.Instance.SetLineRender(transform.position, planet.transform.position);
        enemyPlanetLines.Add(new LineRenderPair(planet, line));
    }

   
    public void ClaimDefend(Planet planet)
    {
        if (planet.owner != null)
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

        for (int i = 0; i < battleUnits.Count; i+=Random.Range(1,3))
        {
            if (battleUnits[i].canDefendOtherPlanet==false || battleUnits[i].isDefending)
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
            //自己
            foreach (var t in enemyPlanetLines)
            {
                Destroy(t.line.gameObject);
            }
            
            return;
        }
        var line = enemyPlanetLines.Find(x => x.planet == planet)?.line;
        if(line)
            Destroy(line.gameObject);
       
    }
    
    void DestroyDefendLine(Planet planet)
    {
        var colonyLine = colonyPlanetLines.Find(x => x.planet == planet)?.line;
        if(colonyLine)
            Destroy(colonyLine.gameObject);
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
    public void SetOwner(Player player)
    {
        this.owner = player;
        planetUi.SetOwner(player);
        ringUi.gameObject.SetActive(false);
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_探索船",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_探索船",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));
        //AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));

        gameObject.name = player.userName;

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
        foreach (var taskCenter in taskCenters)
        {
            taskCenter.Run();
        }
    }

    public override void LogTip(string tip)
    {
        //Debug.Log(tip);
        planetUi.LogTip(tip);
    }


   
    
    public int GetTechLevelByRes()
    {
        int techLevel = 1;
        var techPoint= planetResContainer.GetResNumByType(ResourceType.Tech);
        if (techPoint > 100)
        {
            techLevel = 2;
        }
        
        if (techPoint > 500)
            techLevel = 3;
        
        if (techPoint > 2500)
            techLevel = 4;
        return techLevel;
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

    public void RollSkill()
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
        skillContainer.AddRandomSkill(GetTechLevelByRes());
        planetResContainer.ReduceRes(ResourceType.DicePoint,1);
    }
    
    
    public void UseSkill(int index)
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

        
        var useSuccess=skillContainer.UseSkill(index-1);
        if(useSuccess)
            planetResContainer.ReduceRes(ResourceType.DicePoint,skill.usePoint);
        
            
    }
    
    public void ChangeSkill(int index)
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
        
        skillContainer.ChangeSkill(index-1); 
        planetResContainer.ReduceRes(ResourceType.DicePoint,skill.removePoint+1);
        
        
    }
    
    public void RemoveSkill(int index)
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
        for (int i = 0; i < battleUnits.Count; i++)
        {
            battleUnits[i].Die();
        }
        EventCenter.Broadcast(EnumEventType.OnPlanetDie,this);
        //Destroy(gameObject);

        Destroy(ringUi);
        Destroy(hpUI.gameObject);
        //Destroy(planetUi.gameObject);
        planetUi.UpdateOwnerOnDie();
        //gameObject.SetActive(false);
       
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
        if(gameObject)
            //为新的流程做好准备
            Destroy(gameObject);
    }
    
}
