using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public long uid;
    public string userName;
    
    public string faceUrl;
    public string top_photo;

    public bool die;
    
   
    public Action onGetUrl;
    public Sprite faceSprite;
    public Action onSpriteDownload;
    
    public UserSaveData userSaveData;
    public Action onSetUserData;

    public void SetUserData(UserSaveData target)
    {
        this.userSaveData = target;
        onSetUserData?.Invoke();
    }
    
    
    
    public Player(long uid, string userName, string faceUrl,string top_photo)
    {
        this.uid = uid;
        this.userName = userName;
        
        this.faceUrl = faceUrl;
        this.top_photo = top_photo;
    }
}