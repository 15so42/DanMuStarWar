using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResMgr : MonoBehaviour
{
    public List<ResourceTable> allRes=new List<ResourceTable>();

    public bool HasAnyRes()
    {
        foreach (var resT in allRes)
        {
            if (resT.resourceNum > 0)
                return true;
        }

        return false;
    }
}
