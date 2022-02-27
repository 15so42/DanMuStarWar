using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class CountDownDialogContext : DialogContext
{
    public int duration;
    
    public CountDownDialogContext(int duration)
    {
        this.duration = duration;
    }
}

public class CountDownDialog : Dialog<CountDownDialogContext>
{
    public Text text;
     
    public static void ShowDialog(int duration,Action onClose)
    {
        var dialog = GetShowingDialog(nameof(CountDownDialog)) as CountDownDialog;
        if (dialog != null)
        {
            return;
        }

        DialogUtil.ShowDialogWithContext(nameof(CountDownDialog), new CountDownDialogContext(duration),null,onClose);
    }

    public override void Show()
    {
        
        base.Show();
        UnityTimer.Timer.Register(dialogContext.duration, Close, (time) =>
        {
            text.text = (dialogContext.duration - (int) time)+"";
        } );
    }
}
