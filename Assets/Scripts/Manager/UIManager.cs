using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   private GameManager gameManager;
   public HpCanvas hpCanvas;

   public WaitingJoinUI waitingJoinUi;
   public void Init(GameManager gameManager)
   {
      this.gameManager = gameManager;
   }

   public void ResetUi()
   {
      waitingJoinUi.gameObject.SetActive(true);
      waitingJoinUi.ResetUi();
      Debug.Log("ResetUI待实现");
   }

   public void UpdateWaitingJoinUI(float time)
   {
      waitingJoinUi.UpdateUI(time);
   }

   

   public AccountUI GetAccountUiByPlayer(Player player)
   {
      return null;
   }

   public HpBar CreateHpBar(GameEntity gameEntity)
   {
      return hpCanvas.CreateHpBar(gameEntity);
   }

   public PlanetUI CreatePlanetUI(Planet planet)
   {
      return hpCanvas.CreatePlanetUI(planet);
   }
}
