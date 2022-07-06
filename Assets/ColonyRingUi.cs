using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColonyRingUi : MonoBehaviour
{
    private Camera mainCamera;
    private Planet owner;

    private Vector3 offset;

    public Image ringBG;
    public Image ring;
    
    private void Awake()
    {
        mainCamera=Camera.main;
    }

    public void OnColonyPointChanged(float point,float maxPoint)
    {
        UpdateRing(point,maxPoint);
    }

    private void OnDestroy()
    {
        owner.onColonyPointChanged =null;
        
    }

    public void SetColor(Color color)
    {
        ring.color = color;
    }

    public void UpdateRing(float point,float maxPoint)
    {
        if(Math.Abs(point) < 0.1f)
            ringBG.CrossFadeAlpha(0,1f,true);
        else
        {
            ringBG.CrossFadeAlpha(1,1f,true);
        }
        ring.fillAmount = (float)point / maxPoint;
        
    }

    void UpDatePos()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if(mainCamera==null)
                return;
        }

        if(owner )
            transform.position = mainCamera.WorldToScreenPoint(owner.transform.position)+offset;
    }

 

    private void LateUpdate()
    {
        UpDatePos();
    }

    public void Init(Planet planet,Color color)
    {
        this.owner = planet;
        planet.onColonyPointChanged += OnColonyPointChanged;
        this.offset = planet.ringOffset;
        //this.transform.localScale = gameEntity.hpUIScale;
        var rect = ringBG.GetComponent<RectTransform>();
        var width = rect.rect.width;
        var height = rect.rect.height;
        ringBG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width* planet.ringUiScale.x );
        ringBG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,height* planet.ringUiScale.y );

        ring.color = color;
        //this.hpFill.color = color;
    }
}
