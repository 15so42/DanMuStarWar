using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;

using UnityEngine;
using UnityEngine.Networking;
using UnityTimer;

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
   public static SongHime Instance;
   
   public AudioSource audioSource;
   public int index;//当前播放序号
   public Queue<RequestSongPair> requestSongPairs=new Queue<RequestSongPair>();


   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      audioSource = GetComponent<AudioSource>();
   }

   public void AddSong(RequestSongPair requestSongPair)
   {
      requestSongPairs.Enqueue(requestSongPair);
      if (requestSongPairs.Count == 1)
      {
       
         if (requestSongPair.requestSongType == RequestSongType.Name)
         {
            PlayFirstSong();
         }
      }
   }

   public void PlayFirstSong()
   {
      var requestSongPair = requestSongPairs.Peek();
      
      if (requestSongPair.requestSongType == RequestSongType.Name)
      {
         StartCoroutine(GetSongId(requestSongPair.value, (id)=>
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

   public void OnDownComplete(AudioClip audioClip)
   {
      audioSource.clip = audioClip;
      audioSource.Play();
      Timer.Register(audioClip.length, () =>
      {
         requestSongPairs.Dequeue();
         if (requestSongPairs.Count > 0)
         {
            PlayFirstSong();
         }
      });
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
      }
      else
      {
         var json = request.downloadHandler.text;
         
               
         SongJson ret = JsonMapper.ToObject<SongJson>(json);
         var id = ret.result.songs[0].id;
         action.Invoke(id);
      }

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
         yield return www.SendWebRequest();
         if (www.isNetworkError)
         {
            Debug.Log(www.error);
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
