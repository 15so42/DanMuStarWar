using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResContainer : MonoBehaviour
{
    public List<ResourceTable> allRes=new List<ResourceTable>();

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
}
