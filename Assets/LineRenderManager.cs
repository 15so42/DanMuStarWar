using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using UnityEngine;

public class LineRenderManager : MonoBehaviour
{
    public static LineRenderManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject lineRenderPfb;
    public GameObject colonyLinePfb;

    private GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent=new GameObject("==========LineRenders========");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public LineRenderer SetLineRender(Vector3 startPos, Vector3 endPos,GameObject pfb=null)
    {
        if (pfb == null)
            pfb = lineRenderPfb;
       

        GameObject lineRenderGo = null;
        RecycleAbleObject lineRender=UnityObjectPoolManager.Allocate(pfb.name);
        if(lineRender==null)
            lineRenderGo = GameObject.Instantiate(pfb,parent.transform);
        else
        {
            lineRenderGo = lineRender.gameObject;
        }
        
        var lr = lineRenderGo.GetComponent<LineRenderer>();
        lr.SetPositions(new[]{startPos,endPos});
        return lr;
    }
}
