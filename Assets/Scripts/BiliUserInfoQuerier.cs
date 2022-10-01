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

    public void Query(int uid,Player player)
    {
        GameManager.Instance.StartCoroutine(QueryAccount(uid, player));
    }
    IEnumerator QueryAccount(int uid,Player player)
    {
        //https://tenapi.cn/bilibili/?uid=350692333
        string baseUrl = "https://tenapi.cn/bilibili/?uid=";
        var url = baseUrl  + uid;
            
        UnityWebRequest request = UnityWebRequest.Get(url);
        
        request.timeout = 15;
        yield return request.SendWebRequest();
            
        if(request.isNetworkError || request.isHttpError) {
            Debug.LogError(request.error);
        }
        else
        {
            var json = request.downloadHandler.text;
               
            AccountInfo ret = JsonMapper.ToObject<AccountInfo>(json);
            player.faceUrl = ret.data.avatar;
            //player.top_photo = ret.data.top_photo;
            player.top_photo = "";//用不到获取头图了
            player.onGetUrl?.Invoke();
                
            PhpTester.Instance.GetUserByUid(uid,player.userName, player.SetUserData);
        }
            
            
        
    }

}

public class AccountInfo{
    public int code;
    //public string message;
    
    public Data data;
    
}

public class Data
{
    public string uid;
    public string name;
    public int level;
    public string sex;
    public string description;
    public string avatar;
    //public string top_photo;
}