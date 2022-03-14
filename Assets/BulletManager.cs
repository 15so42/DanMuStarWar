using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using UnityEngine;

public enum BulletType
{
    Normal,
}

[System.Serializable]
public struct BulletConfig
{
    public BulletType type;
    public GameObject pfb;
}
public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<BulletConfig> bulletConfigs=new List<BulletConfig>();

    public GameObject GetBullet(BulletType bulletType)
    {
        var pfb = bulletConfigs.Find(x => x.type == bulletType).pfb;
        var pfbName = pfb.name;
        GameObject go;
        var recycle = UnityObjectPoolManager.Allocate(pfbName);
        go = !recycle ? GameObject.Instantiate(pfb) : recycle.gameObject;
        return go;
    }
    
   
}
