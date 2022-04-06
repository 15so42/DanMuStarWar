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
            
        string baseUrl = "http://api.bilibili.com/x/space/acc/info";
        var url = baseUrl + "?" + "mid=" + uid;
            
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
            player.faceUrl = ret.data.face;
            player.top_photo = ret.data.top_photo;
            player.onGetUrl?.Invoke();
                
        }
            
            
        
    }

}

public class AccountInfo{
    public int code;
    public string message;
    
    public Data data;
    
}

public class Data
{
    public string name;
    public string face;
    public string top_photo;
}