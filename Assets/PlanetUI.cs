﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetUI : MonoBehaviour
{
    public Transform playerArea;
    public Image playerIcon;
    public Text playerName;
    
    public Vector3 offset=Vector3.up;
    
    private Planet owner;

    private Camera mainCamera;
    [Header("UiConfig")] public TMP_Text moneyText;
    public TMP_Text techText;
    public TMP_Text populationText;
    [Header("骰子点数")]
    public TMP_Text dicePointText;


    public Transform skillGroupUI;

    public Transform msgBg;
    public Text msgText;
    
    
    private Player player;
    [HideInInspector]
    public int planetIndex;
    public void SetIndex(int index)
    {
        this.planetIndex = index;
        //playerName.text = "["+index+"]"+player.userName;
    }
    private void Awake()
    {
        mainCamera=Camera.main;
    }

    public void SetOwner(Player player)
    {
        this.player = player;
        playerArea.gameObject.SetActive(true);
        //playerIcon.sprite=player.faceIcon;
        playerName.text = "["+planetIndex+"]"+player.userName;
    }
    
    //资源数量更新
    public void onResChanged(ResourceType resType, int num)
    {
        UpdateResUI();
    }

    void UpdateResUI()
    {
        foreach (var t in owner.planetResContainer.allRes)
        {
            if (t.resourceType == ResourceType.Money)
            {
                moneyText.text = t.resourceNum.ToString();
            }
            if (t.resourceType == ResourceType.Tech)
            {
                techText.text = t.resourceNum.ToString();
            }
            if (t.resourceType == ResourceType.Population)
            {
                populationText.text = t.resourceNum.ToString();
            }
            //
            if (t.resourceType == ResourceType.Tech)
            {
                populationText.text = "LV:"+owner.GetTechLevelByRes()+"";
            }
            if (t.resourceType == ResourceType.DicePoint)
            {
                dicePointText.text = t.resourceNum.ToString();
            }
        }
       

       
    }
    
    

   

    void UpDatePos()
    {
        if(owner)
            transform.position = mainCamera.WorldToScreenPoint(owner.transform.position)+offset;
    }

    private void LateUpdate()
    {
        UpDatePos();
    }

    public void Init(Planet planet)
    {
        this.owner = planet;
        planet.planetResContainer.AddResChangeListener(onResChanged);
        UpdateResUI();
    }

    public void LogTip(string msg)
    {
        
        msgBg.transform.localScale=Vector3.zero;
        msgBg.gameObject.SetActive(true);
        msgText.text = msg;

        msgBg.transform.DOScale(Vector3.one, 1);
        UnityTimer.Timer.Register(2, () =>
        {
            msgBg.gameObject.SetActive(false);
        });

    }
}
