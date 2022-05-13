using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MessageBox : MonoBehaviour
{
    public static MessageBox _instance;

    [Header("=====gameObject=====")]
    public Image bg;
    public Text text;
    public Scrollbar scrollbar;
    
    [Header("隐藏ScrollBar")]
    public Image scrollBarImage;
    public GameObject SlidingArea;

    
    bool isShowing;
    [Header("淡入淡出时间")]
    public float fadeTime = 0.2f;
    [Header("最后一条消息后聊天框消失时间")]
    public float hideAfterLastMessageTime = 6;
    [Header("保留字数")]
    public int remainCharaters = 160;


    float timer=0;


    private void Awake()
    {
        _instance = this;
        //初始隐藏
        HideImmadietly();
       
    }



    private void Update()
    {
        //一定时间后自动隐藏
        timer += Time.deltaTime;
        if (timer > hideAfterLastMessageTime)
        {
            Hide();
        }

        //按tab控制开关
        if (Input.GetKeyDown(KeyCode.Tab))
        {

            if (isShowing)
                Hide();
            else
                Show();
        }
    }


    public void AddMessage(string message)
    {
        Show();
        string t = text.text + message+"\n";

        //截取后面80个字符
        t = t.Substring(t.Length>remainCharaters?t.Length - remainCharaters : 0, t.Length> remainCharaters ? remainCharaters : t.Length);
        scrollbar.value = 0;
        text.text = t;
    }

    public void Hide()
    {
        isShowing = false;

        GetComponent<Image>().CrossFadeAlpha(0, fadeTime, false);
        bg.CrossFadeAlpha(0, fadeTime, false);
        text.CrossFadeAlpha(0, fadeTime, false);
        
        scrollBarImage.enabled = false;
        SlidingArea.SetActive(false);
    }

    //用于初始化
    public void HideImmadietly()
    {
        isShowing = false;

        GetComponent<Image>().CrossFadeAlpha(0, 0, false);
        bg.CrossFadeAlpha(0, 0, false);
        text.CrossFadeAlpha(0, 0, false);

        scrollBarImage.enabled = false;
        SlidingArea.SetActive(false);
    }

    public void Show()
    {
        isShowing = true;

       
        GetComponent<Image>().CrossFadeAlpha(1, fadeTime, false);
        
        bg.CrossFadeAlpha(1, fadeTime, false);
        text.CrossFadeAlpha(1, fadeTime, false);

        scrollBarImage.enabled = true;
        SlidingArea.SetActive(true);
        timer = 0;
    }
}
