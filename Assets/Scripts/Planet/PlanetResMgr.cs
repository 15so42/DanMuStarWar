using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResMgr : MonoBehaviour
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

    public void OnResChanged(ResourceType resType,int num)
    {
        onResChanged.Invoke(resType,num);
    }

    public void AddResChangeListener(Action<ResourceType, int> action)
    {
        onResChanged += action;
    }
}
