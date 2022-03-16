using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class PlanetResContainer : MonoBehaviour
{
    public float tickTime = 20;
    private float timer=20;
    public List<ResourceTable> allRes=new List<ResourceTable>();

    private void Start()
    {
        timer = tickTime;
    }

    //Action
    public Action<ResourceType, int> onResChanged = null;
    public bool HasAnyRes()
    {
        foreach (var resT in allRes)
        {
            if (resT.resourceNum > 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// add
    /// </summary>
    /// <param name="resTable"></param>
    /// <param name="add"></param>
    public void AddRes(ResourceType resourceType,int num)
    {

        ResourceTable resT = allRes.Find(x => x.resourceType == resourceType);
        resT.resourceNum += num;
        UpdateRes(resourceType,num);
    }

    public void ReduceRes(ResourceType resourceType, int num)
    {
        AddRes(resourceType,-1*num);
    }
    public void UpdateRes(ResourceType resourceType,int num)
    {
        
        OnResChanged(resourceType,num);
    }
    public void OnResChanged(ResourceType resType,int num)
    {
        onResChanged.Invoke(resType,num);
    }

    public void AddResChangeListener(Action<ResourceType, int> action)
    {
        onResChanged += action;
    }

    public int GetResNumByType(ResourceType resourceType)
    {
        return allRes.Find(x => x.resourceType == resourceType).resourceNum;
    }

    /// <summary>
    /// 科技，人口自增长
    /// </summary>
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = tickTime;
            var techRes = allRes.Find(x => x.resourceType == ResourceType.Tech);
            techRes.resourceNum = (int)(techRes.resourceNum * 1.1);
            
            var populationRes = allRes.Find(x => x.resourceType == ResourceType.Population);
            populationRes.resourceNum = (int)(populationRes.resourceNum * 1.1);
        }
    }
}
