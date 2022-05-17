using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class McBattleOverDialogContext : DialogContext
{
    public int duration;
    
    
    public GameMode gameMode;
    public List<PlanetCommander> winnerPlanetCommanders;
    public List<PlanetCommander> lostPlanetCommanders;
    
    

    public McBattleOverDialogContext(int duration,GameMode gameMode,List<PlanetCommander> winnerPlanetCommanders,List<PlanetCommander> lostPlanetCommanders) 
    {
        this.duration = duration;
        this.gameMode = gameMode;
        this.winnerPlanetCommanders = winnerPlanetCommanders;
        this.lostPlanetCommanders = lostPlanetCommanders;
    }
}
public class MCBattleOverDialog : Dialog<McBattleOverDialogContext>
{
  
    public TMP_Text countDownText;
    

    public Transform leftGridLayout;
    public Transform rightGridLayout;
    public GameObject statisItemUiPfb;
    
    
    public static void ShowDialog(int duration,GameMode gameMode, List<PlanetCommander> winners,List<PlanetCommander> losers, Action onClose)
    {
        var dialog = GetShowingDialog(nameof(MCBattleOverDialog)) as MCBattleOverDialog;
        if (dialog != null)
        {
            return;
        }


        DialogUtil.ShowDialogWithContext(nameof(MCBattleOverDialog), new McBattleOverDialogContext(duration, gameMode, winners,losers),null,onClose);
    }

    public override void Show()
    {
        base.Show();

        StartCoroutine(CountDown(dialogContext.duration));
       
        dialogContext.winnerPlanetCommanders.Sort((x,y)=>x.attackedDamage>y.attackedDamage?-1:1);
        dialogContext.lostPlanetCommanders.Sort((x,y)=>x.attackedDamage>y.attackedDamage?-1:1);
        
        for (int i = 0; i < ((dialogContext.winnerPlanetCommanders.Count>5) ? 5 :dialogContext.winnerPlanetCommanders.Count); i++)
        {
            var planetCommander = dialogContext.winnerPlanetCommanders[i];
            var playerStatisItemUi = GameObject.Instantiate(statisItemUiPfb, leftGridLayout).GetComponent<PlayerStatisItemUi>();
            
            playerStatisItemUi.Init(planetCommander);
            
        }
        
        for (int i = 0; i < ((dialogContext.lostPlanetCommanders.Count>5) ? 5 :dialogContext.lostPlanetCommanders.Count); i++)
        {
            var planetCommander = dialogContext.lostPlanetCommanders[i];
            var playerStatisItemUi = GameObject.Instantiate(statisItemUiPfb, rightGridLayout).GetComponent<PlayerStatisItemUi>();
            
            playerStatisItemUi.Init(planetCommander);
            
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
