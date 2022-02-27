using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using LitJson;
using UnityEngine;
/// <summary>
/// 根据uid查询bilibil用户信息
/// </summary>
public class BiliUserInfoQuerier : MonoBehaviour
{
    public static AccountInfo Query(int uid)
    {
        string baseUrl = "http://api.bilibili.com/x/space/acc/info";
        var url = baseUrl + "?" + "mid=" + uid;
        
        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        //request.Accept = "application/json, text/javascript, */*; q=0.01";
        //request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
        
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        try
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                var ret= reader.ReadToEnd();
                Debug.Log(ret);
                AccountInfo accountInfo = JsonMapper.ToObject<AccountInfo>(ret);
                return accountInfo;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e+",请求链接为:"+url);
           
        }

        return new AccountInfo(){code=-1,message = "获取用户头像失败"};
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
