using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonPointUi : FollowEntityUi
{
    public Text pointText;


    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(40, 65);
    }

    public void UpdateUi(int point)
    {
        
        pointText.text = point + "";
    }
}
