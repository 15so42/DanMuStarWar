using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DebugDialog : Dialog
{
    public GameObject container;
    public InputField inputField;
    public Button joinButton;
    public Button submitButton;
    public Button upArrow;
    public Button downArrow;
    public Button leftArrow;
    public Button rightArrow;

    public string initName = "Player0";
    public int initUid = 336223980;
    
    public static void ShowDialog()
    {
        DialogUtil.ShowDialog(nameof(DebugDialog));
        
    }

    public override void Show()
    {
        var dialog = GetShowingDialog(nameof(DebugDialog)) as DebugDialog;
        base.Show();
        
        if (dialog != null)
        {
            container.transform.localPosition=new Vector3(380,-206,0);
            initName = "Player1";
            initUid = 23204263;
        }
        else
        {
            container.transform.localPosition=new Vector3(380,206,0);
        }
        
        joinButton.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(false);
        
        joinButton.onClick.AddListener(() =>
        {
            DanMuReciver.Instance.SendFakeDanMu(initName,initUid,"加入游戏");
            joinButton.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(true);
        });
        
        submitButton.onClick.AddListener(() =>
        {
            var message = inputField.text;
            DanMuReciver.Instance.SendFakeDanMu(initName,initUid,message);
        });
        
        upArrow.onClick.AddListener(() =>
        {
            container.transform.DOLocalJump(container.transform.localPosition + Vector3.up * 200, 2, 2, 2);
        });
        downArrow.onClick.AddListener(() =>
        {
            container.transform.DOLocalJump(container.transform.localPosition - Vector3.up * 200, 2, 2, 2);
        });
        rightArrow.onClick.AddListener(() =>
        {
            container.transform.DOLocalJump(container.transform.localPosition + Vector3.right * 300, 2, 2, 2);
        });
        leftArrow.onClick.AddListener(() =>
        {
            container.transform.DOLocalJump(container.transform.localPosition - Vector3.right * 300, 2, 2, 2);
        });
    }
}
