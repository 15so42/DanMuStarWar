using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PVEManager : MonoBehaviour
{

    public static PVEManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private FightingManager fightingManager;

    [Header("难度等级")] public int difficulty;
    

    private List<ZombieSpawner> spawners=new List<ZombieSpawner>();
    private List<string> toSpawnList=new List<string>();

    private UnityTimer.Timer addDiffTimer;
    
    private void Start()
    {
        fightingManager=FightingManager.Instance;
        EventCenter.AddListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.AddListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.AddListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.AddListener(EnumEventType.OnBattleStart,OnBattleStart);
        
        RenderSettings.ambientLight=new Color(0.65f,0.65f,0.65f);
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
                    planetCommanders[i].AddPoint(0.5f);
                }
            }
            
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
        var winners = PlanetManager.Instance.allPlanets[0].planetCommanders;
        FightingManager.Instance.GameOverByMc(winners,null,true);
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
            var spellCount = Mathf.CeilToInt(difficulty/2f);//附魔数量为难度等级/3

            var maxSpellSlot =Mathf.CeilToInt(difficulty/15f)+2;//每15分钟增加一个槽位
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
    public void SpawnByPlayerCount(int count)
    {
        var playerCount = fightingManager.players.Count;
        var rate = ((float) playerCount / 6);
        if (rate < 0.2f)
            rate = 0.2f;
        if (rate > 2f)
            rate = 2f;
        
        var realCount=Mathf.CeilToInt(count * rate );
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
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<McUnit>(EnumEventType.OnMonsterInit,OnMonsterInit);
        //EventCenter.RemoveListener<ZombieSpawner>(EnumEventType.OnMonsterSpawnerInit,OnMonsterSpawnerInit);
        EventCenter.RemoveListener(EnumEventType.OnBattleOver,OnBattleOver);
        EventCenter.RemoveListener(EnumEventType.OnBattleStart,OnBattleStart);
    }
}
