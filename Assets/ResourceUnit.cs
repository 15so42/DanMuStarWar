using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Money,
    Tech,//决定生产队列等级
    Population,//人口，人口决定建造和生产速度。部分星球上有可获取人口(奴役太难听了）
}
[Serializable]
public struct ResourceTable
{
    public ResourceType resourceType;
    public float resourceNum;
}
public class ResourceUnit : MonoBehaviour
{
    public List<ResourceTable> resourceTables=new List<ResourceTable>();

    public ResourceTable GetResource(ResourceType resourceType)
    {
        return resourceTables.Find(x => x.resourceType == resourceType);
    }
    
    public void OnCollect(ResourceType resourceType, int num)
    {
        var resourceTable = GetResource(resourceType);
        resourceTable.resourceNum -= num;
    }
}
