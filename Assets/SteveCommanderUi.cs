using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteveCommanderUi : CommanderUI
{
    [Header("复活倒计时")] public Image fill;

    public Text countDownText;
    
    // Start is called before the first frame update
    void Start()
    {
        fill.gameObject.SetActive(false);
        countDownText.text = "";
    }

    public void BreakCountDown()
    {
        StopAllCoroutines();
        fill.gameObject.SetActive(false);
        countDownText.text = "";
    }

    public void StartCountDown(int time)
    {
        
        StopAllCoroutines();
        StartCoroutine(RespawnCountDown(time));
    }

    IEnumerator RespawnCountDown(int time)
    {
        int count = time;
        fill.gameObject.SetActive(true);
        countDownText.text = time+"";
        while (true)
        {
            yield return new WaitForSeconds(1);
            count--;
            fill.fillAmount = (float) count / time;
            countDownText.text = count + "";
            if (count <= 0)
            {
                fill.gameObject.SetActive(false);
                countDownText.text = "";
                yield break;
            }
               
        }
    }

    public void OnHangUp()
    {
        fill.fillAmount = 1;
        countDownText.text = "挂机";
    }
}
