using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CommanderUI : MonoBehaviour
{
    
    [HideInInspector]public GameObject commanderGo;
    [HideInInspector]public Camera mainCamera;
    [HideInInspector]public PlanetCommander planetCommander;

    //组件拖拽
    public Image face;
    public Text pointText;
    [Header("LogTip")] public Image msgBg;
    public Text msgText;
    
    [Header("配置")] public Vector3 offset;
    
    //UI动销
    
    private Sequence sequence;
    private UnityTimer.Timer timer; 
    private void Awake()
    {
        mainCamera=Camera.main;
    }
    

    void UpDatePos()
    {
        transform.position = mainCamera.WorldToScreenPoint(commanderGo.transform.position)+offset;
    }

    private void LateUpdate()
    {
        UpDatePos();
    }

    public void Init(GameObject commanderGo,PlanetCommander planetCommander)
    {
        this.commanderGo = commanderGo;
        this.planetCommander = planetCommander;
        planetCommander.onPointChanged+=OnPointChanged;
        UpdateUI(planetCommander);
    }

    void UpdateUI(PlanetCommander planetCommander)
    {
        if(commanderGo==null)//因为是延迟设置commander
            return;
        var player = this.planetCommander.player;
        if (this.planetCommander.player.faceSprite == null)
        {
            player.onSpriteDownload += OnSpriteDownload;
            return;
        }
        
        face.sprite = planetCommander.player.faceSprite;
       
    }

    void OnSpriteDownload()
    {
        UpdateUI(planetCommander);
    }

    void UpdateText(int point)
    {
        pointText.text = point + "";
    }

    void OnPointChanged(int point)
    {
        UpdateText(point);

    }

    public void LogTip(string msg)
    {
        timer?.Cancel();
        
        msgBg.transform.localScale=Vector3.zero;
        msgBg.gameObject.SetActive(true);
        msgText.text = msg;
        
        sequence?.Kill();
    
        sequence = DOTween.Sequence();
        sequence?.Append(msgBg.transform.DOScale(Vector3.one, 1));
        timer=UnityTimer.Timer.Register(3, () =>
        {
            msgBg.gameObject.SetActive(false);
        });
    
    }
}
