using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using Ludiq;
using UnityEngine;

public class LineRenderManager : MonoBehaviour
{
    public static LineRenderManager Instance;

//public  List<GameObject> lineGos=new List<GameObject>();
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
        EventCenter.AddListener(EnumEventType.OnStartWaitingJoin,ClearAllLine);
    }

    void ClearAllLine()
    {
        var trans = parent.GetComponentsInChildren<Transform>();
        for (int i = 1; i < trans.Length; i++)
        {
            Destroy(trans[i].gameObject);
        }
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
