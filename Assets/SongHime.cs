using System;
using System.Collections;
using System.Collections.Generic;
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
         StartCoroutine(GetSongId(requestSongPair.value, TryDownload));//获取到歌单id后开始下载歌曲
      }
      else
      {
         StartCoroutine(DownSong(requestSongPair.value, OnDownComplete));
      }
   }

   public void TryDownload(string id)
   {
      string url = $"http://music.163.com/song/media/outer/url?id={id}.mp3";
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
   
   IEnumerator GetSongId(string songName,Action<string> action)
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
         action.Invoke(id.ToString());
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
   {
      using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
      {
         yield return www.SendWebRequest();
         if (www.isNetworkError)
         {
            Debug.Log(www.error);
         }
         else
         {
            AudioClip audioClip = null;
#if UNITY_EDITOR
            //audioClip = NAudioPlayer.FromMp3Data(www.downloadHandler.data);
#else
audioClip = DownloadHandlerAudioClip.GetContent(www);
#endif
            action.Invoke(audioClip);
         }
      }
   }
   
   
}
