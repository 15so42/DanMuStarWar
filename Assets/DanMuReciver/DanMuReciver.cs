using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;


public class DanMuReciver : MonoBehaviour
{
    public static DanMuReciver Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<string> lastMessageStrList = new List<string>();
    

    //弹幕接收器
    string url = "https://api.live.bilibili.com/xlive/web-room/v1/dM/gethistory?roomid=880235";
    HttpWebRequest request;


    //全局变量
    long lastReadUnix;//可能每次检测之间有多条弹幕，从上一次读取时间以后读取之后的所有弹幕
    //int lastReadUid =0;


    public float tickInterval=5f;//轮询间隔
    //Debug
    public bool debugMode;

    public void SetRequest()
    {
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.Accept = "application/json, text/javascript, */*; q=0.01";
        request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
    }

    private UnityTimer.Timer reconnectTimer;
    public string Response()
    {
        string result = null;
        try
        {
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result= reader.ReadToEnd();
            }
        } catch (WebException wex)
        {
            if (wex != null)
            {

                Debug.LogError("网络暂时出现异常，请稍后");

            }
        }

        return result;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ReciveDanMu());
        DontDestroyOnLoad(gameObject);
        string lastReadUnixStr = PlayerPrefs.GetString("lastReadUnix","0");
        lastReadUnix =  Convert.ToInt64(lastReadUnixStr);
    }

    public void ParseDanMu(ResponseResult ret)
    {
        lastReadUnix = Convert.ToInt64(PlayerPrefs.GetString("lastReadUnix","0"));
        //从头读取每条弹幕，直到时间大于上次读取时间，
        for(int i = 0; i < ret.data.room.Count; i++)
        {
            //从头读取每条弹幕，直到时间大于上次读取时间，
            string time= ret.data.room[i].timeline;
            int uid = ret.data.room[i].uid;
            long unix = GetUnixTime(time);
            
            if (unix>=lastReadUnix )//时间大于上一次读取弹幕的时间或者uid不同
            {
                       
                string name = ret.data.room[i].nickname;
                string text = ret.data.room[i].text;
                string str = name + " [" + time + "]: " + text;
                
                //如果上一次读取是最后两次发言是同一秒，就会出现新的读取时重复读取的问题.
                //因此读取时记录最后一秒中的发言，在新的读取循环时判断之前的最后一秒中是否有新的发言，如果是之前的重复发言，则跳过，如果不是，比如说上一次读取两条发言
                //后又有用户在之前的最后一秒发送了第三条弹幕，则从第三条发言开始读取，这也是不能简单的把新循环的时间设置为之前的最后一秒的时间加一秒的原因。
                if (unix == lastReadUnix && lastMessageStrList.Contains(str))
                {
                    continue;
                }

                Debug.Log(str);
                EventCenter.Broadcast(EnumEventType.OnDanMuReceived,name,uid,time,text);
                //更新上一次读取的弹幕时间
                if (unix > lastReadUnix)
                {
                    lastMessageStrList.Clear();
                }
                else
                {
                    Debug.Log("检测到同一秒发言");
                }
                lastMessageStrList.Add(str);//存储最后一秒的uid

                lastReadUnix = unix;
                //lastReadUid = uid;
                PlayerPrefs.SetString("lastReadUnix",lastReadUnix+"");
                
            }
        }
    }

    IEnumerator ReciveDanMu()
    {
        while (true) { 
            SetRequest();
            string json = Response();

            if (json == null)
            {
                yield return new WaitForSeconds(tickInterval);
                //网络获取失败，跳过，三秒后继续获取
                continue;
            }

            if(debugMode)
                Debug.Log(json);

            ResponseResult ret = JsonMapper.ToObject<ResponseResult>(json);

            ParseDanMu(ret);
            
            yield return new WaitForSeconds(tickInterval);
        }
    }

    /// <summary>
    /// 扩展方法, 本地时间转Unix时间; (如 本地时间 "2020-01-01 20:20:10" 转换unix后等于 1577881210)
    /// </summary>
    /// <param name="time">本地时间</param>
    /// <returns>基于秒的10位数</returns>
    public long GetUnixTime(string timeStr)
    {
        DateTime time = Convert.ToDateTime(timeStr);
        return time.ToUniversalTime().Ticks / 10000000 - 62135596800;
    }

    public void SendFakeDanMu(string nickName,int uid,string text)
    {
        string time = DateTime.Now.ToString();
        EventCenter.Broadcast(EnumEventType.OnDanMuReceived,nickName,uid,time,text);
        
    }

    private void OnDestroy()
    {

        
        PlayerPrefs.SetInt("lastReadUnix",(int)lastReadUnix);
    }

    /* // <summary>
    /// 将Unix时间戳转换为dateTime格式
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public  DateTime UnixTimeToDateTime(long time)
    {
        if (time < 0)
            throw new ArgumentOutOfRangeException("time is out of range");
 
        return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(time);
    }*/
}


public class ResponseResult{
    public int code;
    public DanMuJson data;
}

public class DanMuJson
{
    public List<Admin> admin;
    public List<Room> room;
}

public class Admin
{
    public string text;
    public int uid;
    public string nickname;
    public string timeline;
}

public class Room
{
    public string text;
    public int uid;
    public string nickname;
    public string timeline;
    public bool readStatus = false;
}