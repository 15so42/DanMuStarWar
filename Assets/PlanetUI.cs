using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetUI : MonoBehaviour
{
    public Vector3 offset=Vector3.up;
    
    private Planet owner;

    private Camera mainCamera;
    [Header("UiConfig")] public TMP_Text moneyText;
    public TMP_Text techText;
    public TMP_Text populationText;

    public Transform skillGroupUI;
    private void Awake()
    {
        mainCamera=Camera.main;
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
