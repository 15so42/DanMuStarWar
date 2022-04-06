using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

public class SaveDataManager : MonoBehaviour
{
    public PlayerDataTable playerDataTable;
    
    //JSON:存档和读档
    public void SaveByJson()
    {
        PlayerDataTable playerData = GameManager.Instance.fightingManager.playerDataTable;
        string filePath = Path.Combine(Application.streamingAssetsPath, "PlayerDataTable.json");
        //利用JsonMapper将save对象转换为Json格式的字符串
        string saveJsonStr = JsonMapper.ToJson(playerData);
        //将这个字符串写入到文件中
        //创建一个StreamWriter，并将字符串写入文件中
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        //关闭StreamWriter
        sw.Close();

        
    }

    public void LoadByJson()
    {
        GetJsonText("PlayerDataTable", (s) =>
        {
            PlayerDataTable loadData = JsonMapper.ToObject<PlayerDataTable>(s);
            if(loadData==null)
                loadData=new PlayerDataTable();
            GameManager.Instance.fightingManager.playerDataTable = loadData;
        });
    }
    
    
    /// <summary>
    /// 读取streamingAssest文件夹下的
    /// </summary>
    /// <param name="JsonName">json文本的名字</param>
    /// <param name="action">回调方法（string  是需要赋值的字符串）</param>
    public void GetJsonText(string JsonName, Action<string> action)
    {
        StartCoroutine(IGetText(JsonName+".json",action));
    }
    IEnumerator IGetText(string JsonName, Action<string> action)
    {
        UnityWebRequest www = UnityWebRequest.Get(new Uri(Path.Combine(Application.streamingAssetsPath, JsonName)));
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string str = www.downloadHandler.text;
            action?.Invoke(str);
            Debug.LogError("读取到的文件"+str);
        }
    }



}
