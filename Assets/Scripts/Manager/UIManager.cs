using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   private GameManager gameManager;
   public HpCanvas hpCanvas;

   public WaitingJoinUI waitingJoinUi;
   
   public List<PlanetUI> planetUis=new List<PlanetUI>();
   
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

   

   public PlanetUI GetPlanetUiByPlayer(Player player)
   {
      return planetUis.Find(x => x.player == player);
   }

   public HpBar CreateHpBar(GameEntity gameEntity)
   {
      return hpCanvas.CreateHpBar(gameEntity);
   }
   
   public CommanderUI CreateCommanderUi(GameObject go)
   {
      return hpCanvas.CreateCommanderUi(go);
   }

   public ColonyRingUi CreateRingUi(Planet planet)
   {
      return hpCanvas.CreateRingUi(planet);
   }

   public PlanetUI CreatePlanetUI(Planet planet)
   {
      var planetUI=hpCanvas.CreatePlanetUI(planet);
      planetUis.Add(planetUI);
      return planetUI;
   }
}
