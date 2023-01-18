using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class PVEManager : MonoBehaviour
{

    public static PVEManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private FightingManager fightingManager;

    [Header("难度等级")] public float difficulty=1;
    

    private List<ZombieSpawner> spawners=new List<ZombieSpawner>();
    private List<string> toSpawnList=new List<string>();

    private UnityTimer.Timer addDiffTimer;
    [Header("小怪曲线")]
    public AnimationCurve curve0;
    [Header("铁傀儡曲线")]
    public AnimationCurve curve1;
    [Header("凋零曲线")]
    public AnimationCurve curve2;

    public GameObject recoveryBasePfb;

    public Text diffText;


    private bool fasterVersion = false;
    
    private void Start()
    {
        fightingManager=FightingManager.Instance;
        EventCenter.AddListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.AddListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.AddListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.AddListener(EnumEventType.OnBattleStart,OnBattleStart);
        
        EventCenter.AddListener<Steve>(EnumEventType.OnSteveDied,OnSteveDie);
        EventCenter.AddListener<McUnit>(EnumEventType.OnMcUnitDied,OnMcUnitDie);
        
        RenderSettings.ambientLight=new Color(0.65f,0.65f,0.65f);
    }


    public void ReduceDifficulty(float value)
    {
        difficulty -= value;
        if (difficulty < 1)
            difficulty = 1;
    }

    void OnSteveDie(Steve steve)
    {
        Debug.Log("玩家死亡，难度减0.3");
        ReduceDifficulty(0.3f);
    }

    float GetYByCurve(AnimationCurve curve,float x,McUnit mcUnit)
    {
        if (mcUnit.lastAttacker == null) 
        {
            return 0;
        }
        
        return curve.Evaluate(x);
    }
    void OnMcUnitDie(McUnit mcUnit)
    {
        var battleTime = mcUnit.battleTime;
       
        if (mcUnit as Zombie)
        {
            var toAdd = GetYByCurve(curve0, battleTime,mcUnit)*0.5f;
            difficulty += toAdd;
            Debug.Log("小怪死亡，战斗时间"+battleTime+"难度加"+toAdd);
            
        }else if (mcUnit as IronGolem)
        {
            var toAdd=GetYByCurve(curve1, battleTime,mcUnit)*2.5f;
            difficulty += toAdd;
            Debug.Log("腐化铁傀儡死亡，战斗时间"+battleTime+"难度加"+toAdd);
            
        }else if (mcUnit as Wither)
        {
            var toAdd=GetYByCurve(curve2, battleTime,mcUnit)*5f;
            difficulty += toAdd;
            Debug.Log("凋零死亡，战斗时间"+battleTime+"难度加"+toAdd);
        }
        
    }

    private float timer5 = 0;
    private void Update()
    {
        timer5 += Time.deltaTime;
        if (timer5 > 5)
        {
            if (PlanetManager.Instance.allPlanets.Count > 0)
            {
                var planetCommanders = PlanetManager.Instance.allPlanets[0].planetCommanders;
                for (int i = 0; i < planetCommanders.Count; i++)
                {
                    if (fasterVersion)
                    {
                        planetCommanders[i].AddPoint(2f);
                    }
                    else
                    {
                        planetCommanders[i].AddPoint(1f);
                    }
                    
                }
            }

            diffText.text = difficulty.ToString();
            timer5 = 0;
        }
    }

    private PlayableDirector director;
    void OnBattleStart()
    {
        var spawnersGo = GameObject.FindGameObjectsWithTag("ZombieSpawner");
        for (int i = 0; i < spawnersGo.Length; i++)
        {
            spawners.Add(spawnersGo[i].GetComponent<ZombieSpawner>());
        }
        StopAllCoroutines();
        
        director= GameObject.FindWithTag("PlayableDirector").GetComponent<PlayableDirector>();
        director.Play();

        fasterVersion = false;
        
        fightingManager.uiManager.SetGameTimerTextColor(Color.white);
        var rand = UnityEngine.Random.Range(0, 3);
        if (rand == 0)
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(2.5f);
            TipsDialog.ShowDialog("敌人加快了攻势！",null);
            fasterVersion = true;
            fightingManager.uiManager.SetGameTimerTextColor(Color.red);
        }


        difficulty = 0;
        
        addDiffTimer = UnityTimer.Timer.Register(60, () =>
        {
            AddDifficulty(1);
        }, null, true);
    }

    /// <summary>
    /// 用于战胜末影龙或者达到一个半小时后强行结束
    /// </summary>
    public void GameWin()
    {
        if(fightingManager.gameStatus!=GameStatus.Playing)
            return;
        var winners = PlanetManager.Instance.allPlanets[0].planetCommanders;
        FightingManager.Instance.GameOverByMc(winners,null,false,true);
    }
    
    /// <summary>
    /// 用于战胜末影龙或者达到一个半小时后强行结束
    /// </summary>
    public void GameOver()
    {
        if(fightingManager.gameStatus!=GameStatus.Playing)
            return;
        var losers = PlanetManager.Instance.allPlanets[0].planetCommanders;
        FightingManager.Instance.GameOverByMc(null,losers,false,true);
    }


    /// <summary>
    /// 后背隐藏能源，为planet添加回血效果
    /// </summary>
    /// <returns></returns>

    private GameObject rec;//recoveryBase
    public void AddRecoveryToBase()
    {
        var planet = PlanetManager.Instance.allPlanets[0];
        if (planet != null)
        {
            
            SkillManager.Instance.AddSkill("Skill_圣泉_LV4", planet, null);
            TipsDialog.ShowDialog("准备对抗末影龙！",null);
            rec=GameObject.Instantiate(recoveryBasePfb);
            rec.transform.position = fightingManager.mcPosManager.GetPosByIndex(0);
        }
    }
        
    float GetElapsedTime()
    {
        return fightingManager.roundManager.elapsedTime;
    }

    //决定附魔栏位和附魔次数
    void OnMonsterInit(McUnit mcUnit)
    {
        var pos = fightingManager.mcPosManager.GetPosByIndex(0);
        
        if (mcUnit!=null)
        {
            
            mcUnit.GoMCWorldPos(pos, false);

            var weapon = mcUnit.GetActiveWeapon();
            var spellCount = Mathf.CeilToInt(difficulty/1.5f);//附魔数量为难度等级/3

            var maxSpellSlot =Mathf.CeilToInt(difficulty/20f)+2;//每15分钟增加一个槽位
            if (maxSpellSlot > 7)
            {
                maxSpellSlot = 7;
            }
            if (difficulty < 5)
            {
                spellCount = 0;
            }
            weapon.SetMaxSpellCount(maxSpellSlot);
            for (int i = 0; i < spellCount; i++)
            {
                weapon.RandomSpellBySpellCount();
            }

            mcUnit.canSetPlanetEnemy = true;

        }
    }

    void SpawnUnit(string unitName)
    {
        var spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
        spawner.Spawn(unitName);
        
    }

    //增加难度等级，怪物附魔数量根据难度等级确定
    public void AddDifficulty(int value)
    {
        difficulty += value;
    }
    
    //新增难度等级，难度只增加怪物强度,附魔数量和附魔槽数
    //怪物数量在TimeLine中手动指定
    public void SpawnByPlayerCount(int count,bool overrideCount=false
    )
    {
        var playerCount = fightingManager.players.Count;
        var rate = ((float) playerCount / 6);
        if (rate < 0.2f)
            rate = 0.2f;
        if (rate > 2f)
            rate = 2f;
        
        var realCount=Mathf.CeilToInt(count * rate );
        if (overrideCount)
            realCount = count;
        //Debug.Log("生成"+realCount+"个野怪");
        for (int i = 0; i < realCount; i++)
        {
            var spawner = spawners[UnityEngine.Random.Range(0, spawners.Count)];
            spawner.Spawn(toSpawnList[UnityEngine.Random.Range(0, toSpawnList.Count)]);
           
        }
    }
    

    public void AddMonsterToList(string monsterName)
    {
        toSpawnList.Add(monsterName);
    }

    public void SetMonsterList(List<string> newList)
    {
        toSpawnList = newList;
    }

    public void RemoveMonsterToList(string monsterName)
    {
        toSpawnList.Remove(monsterName);
    }
    
    

    void OnBattleOver()
    {
        StopAllCoroutines();
        director.Stop();
        spawners.Clear();
        toSpawnList.Clear();
        addDiffTimer?.Cancel();
        if(rec)
            Destroy(rec);
        difficulty = 0;
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.RemoveListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.RemoveListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.RemoveListener(EnumEventType.OnBattleStart,OnBattleStart);
    }
}
