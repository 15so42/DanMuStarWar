using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int uid;
    //public string userName;
    public int giftPoint;
    public bool opendScore=false;//是否开启了战绩系统
    public int winCount;//胜场
    public int loseCount;//败场
}