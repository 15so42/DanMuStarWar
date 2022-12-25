using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class EmeraldDialogContext : DialogContext
{
    public float value=1f;

    public EmeraldDialogContext(float count)
    {
        this.value = count;
    }
}

public class EmeraldGiftDialog : Dialog<EmeraldDialogContext>
{
    public Text giftText;
    public static void ShowDialog(float count,Action onClose=null)
    {
        var dialog = GetShowingDialog(nameof(EmeraldGiftDialog)) as EmeraldGiftDialog;
        if (dialog != null)
        {
            return;
        }

        DialogUtil.ShowDialogWithContext(nameof(EmeraldGiftDialog), new EmeraldDialogContext(count),null,onClose);
    }

    public override void Show()
    {
        base.Show();
        giftText.text = "获得奖励:绿宝石*" + dialogContext.value;
        Timer.Register(5, Close);
        
        
    }
}
