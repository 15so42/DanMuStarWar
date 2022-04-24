using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GatherUI : MonoBehaviour
{
    public Image ring;
    public Text sponsorText;
    public Text tipText;
    public void Init(Planet target,Planet sponsor,PlanetCommander planetCommander)
    {
        var color = sponsor.planetColor;
        ring.color = color;

        sponsorText.text = planetCommander.player.userName + "发起了一次集结命令";
        if (target.owner != null && sponsor.owner != null)
        {
            //宣战
            
            tipText.text = "兄弟们，和我一起宣战！";
            
        }
        else
        {
            tipText.text = "请求驻守此处！";
        }
        
        
        Camera minCamera=Camera.main;
        Vector3 pos = minCamera.WorldToScreenPoint(target.transform.position);
        transform.position = pos;
        transform.DOShakeScale(1, 1);
        Destroy(gameObject,30f);
    }
    
}
