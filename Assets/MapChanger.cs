using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MapChanger : MonoBehaviour
{
    [System.Serializable]
    public struct MapPair
    {
        public string logicName;
        [Header("主场景")]
        public string baseSceneName;
        public string realSceneName;
    }
    
    public static MapChanger Instance;
    public List<MapPair> mapPairs=new List<MapPair>();
    private Dictionary<string,int> mapVoteCount=new Dictionary<string, int>();
    
    public List<int> votedUid=new List<int>();//每个uid只能投票一次

    public string desireMap = "";


    void SetDesireMap(string value)
    {
        this.desireMap = value;
        MessageBox._instance.AddMessage("系统","下一局地图设置为"+desireMap);
        //SceneManager.sceneLoaded += ClearVoted;
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
            EventCenter.AddListener<string,int,string,string>(EnumEventType.OnDanMuReceived,OnDanMuReceived);
        }
        else
        {
            Destroy(gameObject);
            return;
            
        }

        
        int index = UnityEngine.Random.Range(0, mapPairs.Count);
        //desireMap = mapPairs[index].logicName;
        desireMap = "PVP";
        Debug.Log("随机地图名："+desireMap);
        ChangeMap(desireMap);
    }

    private void OnDanMuReceived(string userName, int uid, string time, string text)
    {
        //如果uid已经投过票
        if (votedUid.Contains(uid))
        {
            MessageBox._instance.AddMessage("系统",userName+"已经投票过了，请不要再投票了");
            return;
        }
        
        text = text.Trim();
        if (text.StartsWith("换地图"))
        {
            var logicName = text.Substring(3);
            if (!IsValidMapName(logicName))
            {
                MessageBox._instance.AddMessage("系统","投票地图名称错误，请检查");
                return;
            }

            if (mapVoteCount.ContainsKey(logicName))
            {
                mapVoteCount[logicName]++;
            }
            else
            {
                mapVoteCount.Add(logicName,1);
            }
            votedUid.Add(uid);
            //投票命令流程完成，开始统计最多票

            var max = 0;
            var targetKv = mapVoteCount.ElementAt(0);
            foreach (var kv in mapVoteCount)
            {
                if (kv.Value > max)
                {
                    max = kv.Value;
                    targetKv = kv;
                }
            }

            string retLogicName = targetKv.Key;
            SetDesireMap(retLogicName);
        }
    }


    public void ClearVoted()
    {
        
        votedUid.Clear();
        mapVoteCount.Clear();
        TipsDialog.ShowDialog("地图投票重置",null);
        
    }

    public bool IsValidMapName(string logicName)
    {
        bool ret = false;
        foreach (var mapPair in mapPairs)
        {
            if (mapPair.logicName.ToLower() == logicName.ToLower())
                ret = true;
        }

        return ret;
    }

    public MapPair GetMapPairByLogicName(string logicName)
    {
        return mapPairs.Find(x => x.logicName == logicName);
    }
    
    

    public void MapVote(int uid,GameMode gameMode)
    {
        if(votedUid.Contains(uid))
            return;
        // if (gameMode == GameMode.Normal)
        //     normalModeCounter++;
        // if (gameMode == GameMode.BattleGround)
        //     battleGroundModeCounter++;
        // if (normalModeCounter > battleGroundModeCounter)
        //     desireMode = GameMode.Normal;
        // else
        // {
        //     desireMode = GameMode.BattleGround;
        // }
        // voted.Add(uid);
        //
        // gameManager.uiManager.UpdateMapVoteUi(normalModeCounter,battleGroundModeCounter);
    }

    public void ChangeMap(string logicName)
    {
        var curBaseMap = SceneManager.GetSceneAt(0).name;
        var pair = GetMapPairByLogicName(logicName);
        var targetBaseScene = pair.baseSceneName;
        if (targetBaseScene != curBaseMap)
        {
            SceneManager.LoadScene(targetBaseScene);//次程序只负责切换主场景，切换主场景后主场景在LoadAdditve对应的场景
        }
        LoadRealMap(logicName);
    }

    // IEnumerator ChangeMapC(string logicName)
    // {
    //     
    // }
    
    public void LoadRealMap(string logicName)
    {
        StartCoroutine(LoadRealMapC(logicName));
    }

    IEnumerator LoadRealMapC(string logicName)
    {
        var curRealMap = "";
        if(SceneManager.sceneCount>1)
            curRealMap=SceneManager.GetSceneAt(1).name;
        var pair = GetMapPairByLogicName(logicName);
        var targetRealMap = pair.realSceneName;
        
        
        if (targetRealMap != curRealMap)
        {
            if (SceneManager.sceneCount > 1 && SceneManager.GetSceneAt(1).isLoaded==true)
            {
                var unload = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
                yield return unload.isDone;
            }

            SceneManager.LoadScene(targetRealMap,LoadSceneMode.Additive);//次程序只负责切换主场景，切换主场景后主场景在LoadAdditve对应的场景
            
        }
        
    }
    
   
}
