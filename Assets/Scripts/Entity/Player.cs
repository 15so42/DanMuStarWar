using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int uid;
    public string userName;
    
    public string faceUrl;
    public string top_photo;
    
   
    public Action onGetUrl;
  
    
    public Player(int uid, string userName, string faceUrl,string top_photo)
    {
        this.uid = uid;
        this.userName = userName;
        
        this.faceUrl = faceUrl;
        this.top_photo = top_photo;
    }
}