using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 根据uid查询bilibil用户信息
/// </summary>
public class BiliUserInfoQuerier : MonoBehaviour
{
    public static BiliUserInfoQuerier Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Query(long uid,Player player)
    {
        GameManager.Instance.StartCoroutine(QueryAccount(uid, player));
    }
    IEnumerator QueryAccount(long uid,Player player)
    {
        //https://tenapi.cn/bilibili/?uid=350692333
        string baseUrl = "https://api.bilibili.com/x/space/app/index?mid=";
        var url = baseUrl  + uid;
            
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        request.timeout = 30;
        yield return request.SendWebRequest();
            
        if(request.isNetworkError || request.isHttpError) {
            Debug.LogError(request.error);
        }
        else
        {
            var json = request.downloadHandler.text;
               
            AccountInfo ret = JsonMapper.ToObject<AccountInfo>(json);
            player.faceUrl = ret.data.info.face;
            //player.top_photo = ret.data.top_photo;
            player.top_photo = "";//用不到获取头图了
            player.onGetUrl?.Invoke();
                
            PhpTester.Instance.GetUserByUid(uid,player.userName, player.SetUserData);
        }
            
            
        
    }

}

public class AccountInfo{
    public int code;
    public string message;
    public int ttl;
    public Data data;
    
}

public class Data
{
    public Info info;
   
}
public class Info{
    public int mid;
    public string name;
    public int level;
    public string sex;
    //public string description;
    public string face;
    //public string top_photo;
}