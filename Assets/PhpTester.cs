using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class UserSaveData
{
    public int uid;
    public string userName;
    public int giftPoint;
    public int winCount;
    public int loseCount;
    public int coin;
    public int killCount;
    public int dieCount;
    public int skinId;
    public string customSkin64Code;

    public UserSaveData(int uid, string userName, int giftPoint, int winCount, int loseCount, int coin, int killCount, int dieCount, int skinId, string customSkin64Code)
    {
        this.uid = uid;
        this.userName = userName;
        this.giftPoint = giftPoint;
        this.winCount = winCount;
        this.loseCount = loseCount;
        this.coin = coin;
        this.killCount = killCount;
        this.dieCount = dieCount;
        this.skinId = skinId;
        this.customSkin64Code = customSkin64Code;
    }
}

public class PhpTester : MonoBehaviour
{

    public static PhpTester Instance;
    [Header("不需要加斜杠")]
    public string ipAddr = "http://101.42.156.31/";
    public string htDocsPath = "McWar";

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Download());
        //StartCoroutine(GetUserByUid(4,"名字"));
        //StartCoroutine(InsertNewUser(2,"ss"));
        //StartCoroutine(UpdateUser(2,"yyyy",2,3,1,1,1,1,1,""));

        // GetUserByUid(7, "默认名称", (user) =>
        // {
        //     Debug.Log(user.uid+","+user.userName);
        // });
    }



    public void GetUserByUid(int uid,string userName,Action<UserSaveData> onParseJson)
    {
        void OnGetUserJson(string json)
        {
            UserSaveData jsonUserSaveData = JsonUtility.FromJson<UserSaveData>(json);
            onParseJson.Invoke(jsonUserSaveData);
        }

        StartCoroutine(GetUserByUidC(uid, userName,OnGetUserJson));
    }

   

    /// <summary>
    /// 输入php名称，不要斜杠，如GetuserByUid.php
    /// </summary>
    /// <returns></returns>
    public string GetPath(string phpName)
    {
        return "http://" + ipAddr + "/" + htDocsPath+ "/" + phpName;
    }

    IEnumerator GetUserByUidC(int uid,string userName,Action<string> onGetJson)
    {
        WWWForm wwwForm=new WWWForm();
        wwwForm.AddField("uid",uid);
        wwwForm.AddField("userName",userName);

        var path = GetPath("GetUserByUid.php");
        using (UnityWebRequest www = UnityWebRequest.Post( GetPath("GetUserByUid.php") ,wwwForm))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var result = www.downloadHandler.text;

                if (result.StartsWith("Error"))
                {
                    Debug.LogError("获取"+uid+"玩家信息错误");
                    yield break;
                }

                if (result.StartsWith("New record created successfully"))
                {
                    //没有玩家，插入了新玩家
                    StartCoroutine(GetUserByUidC(uid, userName,onGetJson));
                    yield break;
                }
                //成功获取到玩家信息
                Debug.Log(result);
                onGetJson.Invoke(result);

                
            }
        }
    }

   
    public void UpdateUser(UserSaveData userSaveData)
    {
        StartCoroutine(UpdateUserC(userSaveData.uid, userSaveData.userName, userSaveData.giftPoint,
            userSaveData.winCount, userSaveData.loseCount
            , userSaveData.coin, userSaveData.killCount, userSaveData.dieCount, userSaveData.skinId));
    }
    
    IEnumerator UpdateUserC(int uid,string userName,int giftPoint,int winCount,int loseCount,int coin,int killCount,int dieCount,int skinId)
    {
        WWWForm wwwForm=new WWWForm();
        wwwForm.AddField("uid",uid);
        wwwForm.AddField("userName",userName);
        wwwForm.AddField("giftPoint",giftPoint);
        wwwForm.AddField("winCount",winCount);
        wwwForm.AddField("loseCount",loseCount);
        wwwForm.AddField("coin",coin);
        wwwForm.AddField("killCount",killCount);
        wwwForm.AddField("dieCount",dieCount);
        wwwForm.AddField("skinId",skinId);
        //
        
        using (UnityWebRequest www = UnityWebRequest.Post(GetPath("UpdateUser.php") ,wwwForm))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                byte[] results = www.downloadHandler.data;
            }
        }
    }
}
