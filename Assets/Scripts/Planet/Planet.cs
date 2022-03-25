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
    
    public Transform spawnPoint;
    
    //LineRenders
    public List<LineRenderPair> enemyPlanetLines=new List<LineRenderPair>();
    public List<LineRenderPair> colonyPlanetLines=new List<LineRenderPair>();
    private List<LineRenderer> lineRenderers=new List<LineRenderer>();
    
    //自己单位管理
    public List<BattleUnit> battleUnits=new List<BattleUnit>();

    [Header("移除技能需要的骰子点数")]
    public int removeDicePoint = 2;
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
        
        
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetCreated,OnPlayerCreated);
        
        EventCenter.AddListener<BattleUnit>(EnumEventType.OnBattleUnitCreated,OnBattleUnitCreated);
        EventCenter.AddListener<Planet>(EnumEventType.OnPlanetDie,DestroyWarLine);
        
        
       
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
        }
        enemyPlanets.Add(planet);
        enemyPlayers.Add(planet.owner);
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
        colonyPlanets.Add(planet);

        for (int i = 0; i < battleUnits.Count; i+=Random.Range(1,3))
        {
            if (battleUnits[i].ownerPlanet == planet || battleUnits[i].canDefendOtherPlanet==false)
            {
                continue;
            }
            battleUnits[i].ChangeOwnerPlanet(planet);
        }
        var line = LineRenderManager.Instance.SetLineRender(transform.position, planet.transform.position,LineRenderManager.Instance.colonyLinePfb);
        colonyPlanetLines.Add(new LineRenderPair(planet, line));
    }

    void DestroyWarLine(Planet planet)
    {
        var line = enemyPlanetLines.Find(x => x.planet == planet)?.line;
        if(line)
            Destroy(line.gameObject);
        var colonyLine = colonyPlanetLines.Find(x => x.planet == planet)?.line;
        if(colonyLine)
            Destroy(colonyLine.gameObject);
    }

    public void SetIndex(int index)
    {
        planetIndex = index;
        planetUi.SetIndex(planetIndex);
    }
    void OnPlayerCreated(Planet planet)
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
        
        Destroy(hpUI.gameObject);
        Destroy(planetUi.gameObject);
        gameObject.SetActive(false);
       
    }

    public override GameEntity GetAttackerOwner()
    {
        return this;
    }

    public override GameEntity GetVictimOwner()
    {
        return this;
    }

   
    
}
