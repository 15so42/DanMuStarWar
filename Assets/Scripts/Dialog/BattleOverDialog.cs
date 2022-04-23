using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleOverDialogContext : DialogContext
{
    public int duration;
    public Player player;
    public GameMode gameMode;
    public List<PlanetCommander> planetCommanders;
    
    

    public BattleOverDialogContext(int duration, Player player,List<PlanetCommander> planetCommanders) 
    {
        this.duration = duration;
        this.player = player;
        this.planetCommanders = planetCommanders;
    }
}
public class BattleOverDialog : Dialog<BattleOverDialogContext>
{
    public TMP_Text winnerText;
    [Header("border和container用来和棋时隐藏")]
    public Image faceBorder;
    public Transform faceContainer;
    public Image face;
    public TMP_Text countDownText;
    [Header("和棋Text")]
    public TMP_Text drawText;

    public Transform gridLayout;
    public GameObject commanderUiPfb;
    
    
    public static void ShowDialog(int duration,Player player,List<PlanetCommander> planetCommanders, Action onClose)
    {
        var dialog = GetShowingDialog(nameof(BattleOverDialog)) as BattleOverDialog;
        if (dialog != null)
        {
            return;
        }


        DialogUtil.ShowDialogWithContext(nameof(BattleOverDialog), new BattleOverDialogContext(duration, player,planetCommanders),null,onClose);
    }

    public override void Show()
    {
        base.Show();

        StartCoroutine(CountDown(dialogContext.duration));
        // if(dialogContext.player!=null){
        //     drawText.gameObject.SetActive(false);
        //   
        //     winnerText.text = "胜者: " + dialogContext.player.userName;
        //     face.sprite = FightingManager.Instance.uiManager.waitingJoinUi.GetPlayerIconUiByUid(dialogContext.player.uid).faceImg.sprite;
        // }
        // else
        // {
        //     faceBorder.gameObject.SetActive(false);
        //     faceContainer.gameObject.SetActive(false);
        //     winnerText.gameObject.SetActive(false);
        //     face.gameObject.SetActive(false);
        //     drawText.gameObject.SetActive(true);
        //     
        // }
        for (int i = 0; i < dialogContext.planetCommanders.Count; i++)
        {
            var planetCommander = dialogContext.planetCommanders[i];
            var commanderUi = GameObject.Instantiate(commanderUiPfb, gridLayout).GetComponent<CommanderUI>();
            commanderUi.Init(null,planetCommander);
            commanderUi.HidePointUi();
        }
        
    }
    
   

    IEnumerator CountDown(int duration)
    {
        int count = duration;
        while (count > 0)
        {
            
            countDownText.text = count+"秒后开始下一局游戏";
            count--;
            yield return new WaitForSeconds(1);
            
        }
        Close();
        
    }
}
