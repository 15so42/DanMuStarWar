using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityTimer;
using Random = System.Random;

public enum RequestSongType
{
   Name,
   Id,
}
public class RequestSongPair
{
   public RequestSongType requestSongType;
   public string value;
}

public class SongJson
{
   public Result result;
   public int code;
}

public class Result
{
   public List<Song> songs;
   public int songCount;

}
public class Song
{
   public int id;
   public string name;
}
//获取实际下载地址
public class SongUrl
{
   public List<SongUrlData> data;
   public int code;
}

public class SongUrlData
{
   public int id;
   public string url;
}


public class SongHime : MonoBehaviour
{
   
   public List<string> defaultSongs=new List<string>();
   public static SongHime Instance;
   
   public AudioSource audioSource;
   public int index;//当前播放序号
   public Queue<RequestSongPair> requestSongPairs=new Queue<RequestSongPair>();

   public Coroutine playCoroutine;
   public Text songHimeText;
   
   //记录audioSource状态来判断是否播放完毕
   public bool lastPlay = false;

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      audioSource = GetComponent<AudioSource>();
      AddRandomSong();
   }

   void AddRandomSong()
   {
      AddSong(new RequestSongPair(){requestSongType = RequestSongType.Name,value =  defaultSongs[UnityEngine.Random.Range(0,defaultSongs.Count)]});
   }
   
   public void UpdateText(float progress=0)
   {
      var str = "";
      for (int i = 0; i < requestSongPairs.Count; i++)
      {
         if (i == 0)
         {
            str += "["+i+"]"+requestSongPairs.ElementAt(i).value+"["+progress+"%]"+ "\n";
         }else
            str += "["+i+"]"+requestSongPairs.ElementAt(i).value+"\n";
         
      }

      songHimeText.text = str;
      songHimeText.text = songHimeText.text.Replace ("\\n", "\n");  
   }

   public void AddSong(RequestSongPair requestSongPair)
   {
      requestSongPairs.Enqueue(requestSongPair);
      UpdateText();
      if (requestSongPairs.Count == 1)
      {
       
         if (requestSongPair.requestSongType == RequestSongType.Name)
         {
            PlayFirstSong();
         }
      }
   }

  
   public void NextSong()
   {
      Debug.Log("下一曲。。。。。。");
      StopAllCoroutines();
      
      if (requestSongPairs.Count > 0)
      {
         
         requestSongPairs.Dequeue();
         if (requestSongPairs.Count > 0)
         {
            PlayFirstSong();
            UpdateText();
            return;
         }
            
        
      }
      AddRandomSong();
      
      UpdateText();
     
   }

   public void PlayFirstSong()
   {
      if(requestSongPairs.Count==0)
         return;
      var requestSongPair = requestSongPairs.Peek();
      
      if (requestSongPair.requestSongType == RequestSongType.Name)
      {
         playCoroutine=StartCoroutine(GetSongId(requestSongPair.value, (id)=>
         {
            
            StartCoroutine(GetSongUrlById(id,TryDownload));
         }));//获取到歌单id再获取下载链接，获取到下载链接后再下载
      }
      else
      {
        TryDownload(requestSongPair.value);
      }
   }

   public void TryDownload(string url)
   {
     
      Debug.Log("歌曲下载链接:"+url);
      StartCoroutine(DownSong(url, OnDownComplete));
   }

   private void Update()
   {
      if (audioSource.isPlaying != lastPlay)
      {
         if(!audioSource.isPlaying)
            OnPlayComplete();
         lastPlay = audioSource.isPlaying;
      }
   }

   public void OnDownComplete(AudioClip audioClip)
   {
      UpdateText(100);
      audioSource.clip = audioClip;
      audioSource.Play();
      Debug.Log("开始播放，歌曲长度为："+audioClip.length);
      
      
   }
   
   IEnumerator GetSongId(string songName,Action<int> action)
   {
      var url =
         $"http://music.eleuu.com/search?keywords={songName}";
      UnityWebRequest request = UnityWebRequest.Get(url);
      request.SetRequestHeader("Content-Type", "application/json");
      request.timeout = 15;
      yield return request.SendWebRequest();
            
      if(request.isNetworkError || request.isHttpError) {
         Debug.LogError(request.error);
         NextSong();
      }
      else
      {
         var json = request.downloadHandler.text;
         
               
         SongJson ret = JsonMapper.ToObject<SongJson>(json);
         if (ret.result.songs == null)
         {
            TipsDialog.ShowDialog("找不到歌曲:"+songName,null);
            NextSong();
            yield break;
         }
            
         var id = ret.result.songs[0].id;
         action.Invoke(id);
      }

   }

   public void OnPlayComplete()
   {
      Debug.Log("播放完成，下一首");
         
      NextSong();
   }
   
   IEnumerator GetSongUrlById(int id,Action<string> action)
   {
      var url =
         $"http://music.eleuu.com/song/url?id={id}";
      UnityWebRequest request = UnityWebRequest.Get(url);
      request.SetRequestHeader("Content-Type", "application/json");
      request.timeout = 15;
      yield return request.SendWebRequest();
            
      if(request.isNetworkError || request.isHttpError) {
         Debug.LogError(request.error);
         NextSong();
      }
      else
      {
         var json = request.downloadHandler.text;
         
               
         SongUrl ret = JsonMapper.ToObject<SongUrl>(json);
         var songUrl = ret.data[0].url;
         action.Invoke(songUrl);
      }

   }
   
   public void RequestSongByName(string songName)
   {
     
      AddSong(new RequestSongPair() {requestSongType = RequestSongType.Name, value = songName});
   }

   public void RequestSongById(string songId)
   {
      AddSong(new RequestSongPair(){requestSongType = RequestSongType.Id,value = songId});
   }


  
   
   
   IEnumerator DownSong(string url,Action<AudioClip> action)
   {//http://music.163.com/song/media/outer/url?id=1859245776.mp3
      //http://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=1024&client=tw-ob&q=+%22+%20%22Hello%20how%20are%20you%22%20+%20%22&tl=En-gb
      using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
      {
         www.SendWebRequest();
         while (!www.isDone)
         {
            UpdateText(www.downloadProgress*100);
            yield return 1;
         }
         if (www.isNetworkError)
         {
            Debug.Log(www.error);
            Debug.Log("下载歌曲失败，下一首");
            NextSong();
         }
         else
         {
         
            AudioClip audioClip = null;

            
            audioClip = DownloadHandlerAudioClip.GetContent(www);
            action.Invoke(audioClip);
         }
      }
   }
   
   
}
