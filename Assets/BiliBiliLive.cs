using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleBilibiliDanmakuClient.Clients;
using SimpleBilibiliDanmakuClient.Models;
using System;
using System.Threading.Tasks;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
public struct DanMuMsg
{
    public int userId;
    public string userName;
    public string content;
}

[System.Serializable]
public struct GiftMsg
{
    public int userId;
    public string userName;
    public int num;
    public string giftName;
    public int totalCoin;
}
public class BiliBiliLive : MonoBehaviour
{
    public List<DanMuMsg> danMuMsgs=new List<DanMuMsg>();
    public int lastDanMuCount = 0;//记录DanMuMsgs列表长度，update中检测长度，当长度发生变化时分发事件
    
    public List<GiftMsg> giftMsgs=new List<GiftMsg>();
    public int lastGiftCount = 0;//记录GiftMsg列表长度，update中检测长度，当长度发生变化时分发事件
    
    public int roomId;
    // Start is called before the first frame update
    async void Start()
    {
        TcpDanmakuClientV2 client=new TcpDanmakuClientV2();
        await client.ConnectAsync(roomId);
        client.HeartbeatInterval = TimeSpan.FromSeconds(25);
        
        client.ReceivedMessageEvt += OnReceivedMessage;
        client.ReceivedPopularityEvt += OnReceivedPopularity;
        client.DisconnectedEvt += OnDisconnect;
        await Task.Delay(-1);
    }

    private Task OnReceivedMessage(IDanmakuClient client, ReceivedMessageEventArgs e)
    {
        string m = e.Message.ToString();
        JObject jObject=JsonConvert.DeserializeObject(m) as JObject;
        string type = jObject["cmd"].ToString();
        switch (type)
        {
            case "DANMU_MSG":
                //Debug.Log(e.Message);
                int userId = jObject["info"][2][0].ToObject<int>();
                string userName = jObject["info"][2][1].ToString();
                string content = jObject["info"][1].ToString();
                
                Debug.Log($"[{userId}]{userName}:{content}");
                danMuMsgs.Add(new DanMuMsg(){userId= userId,userName = userName,content = content});
                
                break;
            case "SEND_GIFT":
                
                //Debug.Log(e.Message);
                OnReceivedGift(jObject);
                break;
        }
        
        return Task.CompletedTask;
        //throw new NotImplementedException();
    }

    public Task OnDisconnect(IDanmakuClient client, DisconnectedEventArgs args)
    {
        Debug.LogError("断连"+args.ToString());
        
        Start();
        Debug.Log("重连");
        return Task.CompletedTask;
    }


    void OnReceivedGift(JObject obj)
    {
        Debug.Log(obj.ToString());

        int userId = 0;
        string userName = "";
        string giftName = "";
        int num = 0;
        int totalCoin = 0;

        var data = obj["data"];
        var batch = data["batch_combo_send"];
        if (true /*|| batch == null*/)//连击
        {
            userId = data["uid"].ToObject<int>();
            userName = data["uname"].ToString();
            giftName = data["giftName"].ToString();
            num = data["num"].ToObject<int>();
            totalCoin = data["total_coin"].ToObject<int>();
        }
       
       
         
        Debug.Log($"收到来自[{userId}]{userName}的{num}个{giftName}共{totalCoin}个电池");
        
        giftMsgs.Add(new GiftMsg()
        {
            userId = userId,
            userName = userName,
            giftName = giftName,
            num=num,
            totalCoin=totalCoin,
        });
        Debug.Log("add后长度为：" + giftMsgs.Count);
    }



    private Task OnReceivedPopularity(IDanmakuClient client, ReceivedPopularityEventArgs e)
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //"收到来自[23204263]云ぃ空的1个小花花"
            giftMsgs.Add(new GiftMsg(){userId = 23204263,userName = "远空",num=1,giftName = "小花花",totalCoin = 100});
        }
        var danMuMsgsCount = danMuMsgs.Count;//注意因为获取弹幕是异步的，所以这里需要一个确切的值
        if (danMuMsgsCount > lastDanMuCount)
        {
            for (int i = lastDanMuCount; i < danMuMsgsCount; i++)
            {
                EventCenter.Broadcast(EnumEventType.OnDanMuReceived, danMuMsgs[i].userName, danMuMsgs[i].userId, "0", danMuMsgs[i].content);
            }

            lastDanMuCount = danMuMsgsCount;
        }
        
        //礼物同理
        var giftMsgsCount = giftMsgs.Count;//注意因为获取弹幕是异步的，所以这里需要一个确切的值
        if (giftMsgsCount > lastGiftCount)
        {
            for (int i = lastGiftCount; i < giftMsgsCount; i++)
            {
                EventCenter.Broadcast(EnumEventType.OnGiftReceived, giftMsgs[i].userId, giftMsgs[i].userName, giftMsgs[i].num, giftMsgs[i].giftName,giftMsgs[i].totalCoin);
            }

            lastGiftCount = giftMsgsCount;
        }
    }
}
