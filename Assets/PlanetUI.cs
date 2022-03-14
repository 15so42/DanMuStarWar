using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public Transform skillGroupUI;

    
    private Player player;
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
    }
}
