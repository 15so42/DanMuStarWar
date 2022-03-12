using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using Ludiq;
using UnityEngine;



public class Planet : GameEntity
{
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

    [Header("PlanetUI")] public PlanetUI planetUi;

    public Color planetColor;
    
   
    
    
    void Awake()
    {
        base.Awake();
        
        planetCommander = GetComponent<PlanetCommander>();
        taskCenters = GetComponents<TaskCenter>();
        
        planetResContainer = GetComponent<PlanetResContainer>();
        
       
        
      
        
        //事件绑定
        //任意玩家加入游戏均设置为自己的敌人，除非后期主动结盟
        EventCenter.AddListener<Player>(EnumEventType.OnPlayerJoined,OnPlayerJoined);
        
        
       
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
        //SkillManager.Instance.AddSkill("Skill_腐蚀_LV1",this);
    }

  


    void OnPlayerJoined(Player newPlayer)
    {
        if (owner!=null && newPlayer != owner)
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
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_探索船",5)));
        AddTask(new PlanetTask(new TaskParams(TaskType.Create,"BattleUnit_战斗机",5)));
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position,radius);
    }

    public Player GetOwner()
    {
        return owner;
    }

    private void AddTask(PlanetTask planetTask)
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
        Debug.Log(tip);
    }

    
}
