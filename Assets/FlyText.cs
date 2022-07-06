using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using UnityEngine;

public class FlyText : MonoBehaviour
{
    public static FlyText Instance;

    private Transform canvas;
    private Camera mainCamera;

    public Camera MainCamera
    {
        //习惯上属性大写 字段小写

        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            return mainCamera;
        }
    }


    private void Awake()
    {
        Instance = this;
        mainCamera=Camera.main;
    }

    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Main Canvas").transform;
    }

    public GameObject prefab;


    public void Show(Vector3 worldPos,string msg,Color color)
    {
        RecycleAbleObject go = UnityObjectPoolManager.Allocate(prefab.name);
        if (go == null)
            go = GameObject.Instantiate(prefab).GetComponent<RecycleAbleObject>();
        go.transform.SetParent(canvas);
        go.transform.position = MainCamera.WorldToScreenPoint(worldPos);
        go.GetComponent<FlyTextUi>().Init(msg,color);
        go.transform.DOJump(go.transform.position + Vector3.up * 50, 1, 1, 1f).OnComplete(() =>
        {
            go.Recycle();
        });
    }

    public void ShowDamageText(Vector3 worldPos,string msg)
    {
        Show(worldPos,msg,new Color(1,1,1));
    }
    
    //public void ShowTipText()

    public void ShowHealText(Vector3 worldPos,string msg)
    {
        Show(worldPos,msg,new Color(0,1f,0));
    }
}
