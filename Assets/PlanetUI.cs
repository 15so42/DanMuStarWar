using System;
using System.Collections;
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

    public TMP_Text giftPoint;

    //战场模式
    public Image battleGroundModeBg;
    public GameObject giftArea;

    public Transform skillGroupUI;

    public Transform msgBg;
    public Text msgText;
    public Transform skillContainer;

    //UI动效
    private Sequence sequence;
    private UnityTimer.Timer timer; 
    
    public Player player;
    [HideInInspector]
    public int planetIndex;

    public string nameLabel;
    
    //游戏模式
    private GameMode gameMode;
    public void SetIndex(int index)
    {
        this.planetIndex = index;
        //playerName.text = "["+index+"]"+player.userName;
    }
    private void Awake()
    {
        mainCamera=Camera.main;
    }

    public void SetOwner(Player player,GameMode gameMode=GameMode.Normal)
    {
        this.player = player;
        playerArea.gameObject.SetActive(true);
        //playerIcon.sprite=player.faceIcon;
        playerName.text = "["+planetIndex+"]"+player.userName;
        if (GameManager.Instance.fightingManager.gameMode == GameMode.BattleGround)
        {
            playerName.text = "["+planetIndex+"]"+"主星";
        }
        skillContainer.gameObject.SetActive(true);
        UpdatGiftPointUI();

        this.gameMode = gameMode;
        battleGroundModeBg.gameObject.SetActive(false);
        giftArea.gameObject.SetActive(true);
        if (gameMode == GameMode.BattleGround)
        {
            battleGroundModeBg.gameObject.SetActive(true);
            giftArea.gameObject.SetActive(false);
        }
    }

    public void UpdateNameLabel(string s)
    {
        nameLabel = s;
        playerName.text = s;
    }

    private void Start()
    {
        
        if (owner.planetResContainer.GetResNumByType(ResourceType.Population) == 0)
        {
            
            playerName.text = "["+planetIndex+"]无人星球";
            //skillContainer.gameObject.SetActive(false);
        }

        else
        {
            if (gameMode == GameMode.BattleGround)
            {
                playerName.text = "[" + planetIndex + "]无人星球" ;
            }
            else
            {
                playerName.text = "[" + planetIndex + "]" + "(可加入)";
            }
            
        }
        //skillContainer.gameObject.SetActive(false);
    }

    //资源数量更新
    public void onResChanged(ResourceType resType, int num)
    {
        UpdateResUI();
    }

    

    public void UpdatGiftPointUI()
    {
        if(owner.owner==null)
            return;
        //var data = FightingManager.Instance.playerDataTable.FindByUid(owner.owner.uid);
        //giftPoint.text = data.giftPoint.ToString();
        //Debug.Log("礼物UI尚未实现");
    }

    void UpdateResUI()
    {
        foreach (var t in owner.planetResContainer.allRes)
        {
            if (t.resourceType == ResourceType.Money)
            {
                moneyText.text = t.resourceNum.ToString();
            }
            
            if (t.resourceType == ResourceType.Population)
            {
                populationText.text = t.resourceNum.ToString();
            }
            //
            if (t.resourceType == ResourceType.Tech)
            {
                techText.text = t.resourceNum+"["+owner.GetTechLevelByRes()+"]";
            }
            if (t.resourceType == ResourceType.DicePoint)
            {
                dicePointText.text = t.resourceNum.ToString();
            }
        }
       

       
    }

    public void UpdateOwnerOnDie()
    {
        nameLabel="["+planetIndex+"][荒废]"+(player==null?"无人星球":player.userName+"");
        UpdateNameLabel(nameLabel);
        //skillContainer.gameObject.SetActive(false);
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

    public void Init(Planet planet,GameMode gameMode)
    {
        this.gameMode = gameMode;
        this.owner = planet;
        planet.planetResContainer.AddResChangeListener(onResChanged);
        UpdateResUI();
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
        timer=UnityTimer.Timer.Register(2, () =>
        {
            if(gameObject)
                msgBg.gameObject.SetActive(false);
        });

    }
}
