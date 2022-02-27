using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AccountUI : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    public Image faceImg;
    public Image topBg;

    public TMP_Text nickName;

    public TMP_Text winRate;
    public Image fillImage;
    public TMP_Text countDownText;

    public GameObject chatLayout;
    public GameObject chatPfb;

    private FightingManager fightingManager;

    public float chatItemXOffset = 0;
    
    //协程变量
    Coroutine countDownCoroutine;
    
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        fillImage.fillAmount = 0;
        countDownText.text = "";
        nickName.text = "虚位以待";
        winRate.text = "";
        fightingManager = GameManager.Instance.fightingManager;
    }

    public void OnPlayerJoined(Player player)
    {
        nickName.text = player.userName;
        StartCoroutine(DownSprite(player.faceUrl,faceImg));
        StartCoroutine(DownSprite(player.top_photo ,topBg));
        winRate.text = "胜率：5/10";
        this.player = player;

    }

    public Sprite GetFaceSprite()
    {
        return faceImg.sprite;
    }

    public void RoundOver()
    {
        if(countDownCoroutine!=null)
            StopCoroutine(countDownCoroutine);
        fillImage.fillAmount = 0;
        countDownText.text = "";
        
    }

    public void StartNewRound()
    {
        //Debug.Log(player.userName+"开始新回合");
        fillImage.fillAmount = 1;
        countDownText.text = fightingManager.roundDuration+"";
        countDownCoroutine=StartCoroutine(CountDown(fightingManager.roundDuration));
    }

    IEnumerator CountDown(int duration)
    {
        int count = duration;
        while (count > 0)
        {
            fillImage.fillAmount = (float)count / duration;
            countDownText.text = count.ToString();
            count--;
            yield return new WaitForSeconds(1);
            
        }
        RoundOver();//UI只负责显示，跳转到对面回合在RoundManager里跳转
    }

    public void ShowMessage(string message)
    {
        var chatItem = GameObject.Instantiate(chatPfb, chatLayout.transform);
        chatItem.transform.localPosition = chatItem.transform.localPosition+ Vector3.right * chatItemXOffset;
        
        var animator = chatItem.gameObject.GetComponent<Animator>();
        var text = chatItem.transform.GetComponentInChildren<Text>();
        text.text = message;
        
        UnityTimer.Timer.Register(10, ()=>
        {
            animator.Play("FadeOut");
            Destroy(chatItem,1);
        });
        
        RectTransform rectTransform = chatItem.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
    
    IEnumerator DownSprite(string url,Image image)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texD1 = new DownloadHandlerTexture(true);
        wr.downloadHandler = texD1;
        yield return wr.SendWebRequest();
        int width = 1920;
        int high = 1080;
        if (!wr.isNetworkError)
        {
            Texture2D tex = new Texture2D(width, high);
            tex = texD1.texture;
            //保存本地          
            Byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/02.png", bytes);
             
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            
        }
    }
    
}
