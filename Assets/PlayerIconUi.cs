using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerIconUi : MonoBehaviour
{
    
    [HideInInspector]
    public Player player;
    public Image faceImg;
    

    public Text nickName;

   

    private FightingManager fightingManager;

    

    public void Init(Player player)
    {
        OnPlayerJoined(player);
        this.player = player;
    }

    public void OnPlayerJoined(Player player)
    {
        nickName.text = player.userName;
        this.player = player;
        
            
            player.onGetUrl += () =>
            {
                FightingManager.Instance.StartCoroutine(DownSprite(player.faceUrl, faceImg, (sprite) =>
                {
                    player.faceSprite = sprite;
                    player.onSpriteDownload?.Invoke();
                }));
                //StartCoroutine(DownSprite(player.top_photo, topBg));
            };
       
    }
    
    IEnumerator DownSprite(string url,Image image,Action<Sprite> action)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texD1 = new DownloadHandlerTexture(true);
        wr.downloadHandler = texD1;
        yield return wr.SendWebRequest();
        int width = 1920;
        int high = 1080;
        if (!wr.isNetworkError)
        {
            Texture2D tex = new Texture2D(width, high);
            tex = texD1.texture;
            //保存本地          
            Byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/02.png", bytes);
             
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            action.Invoke(sprite);
        }
    }



}
