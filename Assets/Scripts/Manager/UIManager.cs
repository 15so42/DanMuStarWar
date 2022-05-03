using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   private GameManager gameManager;
   public HpCanvas hpCanvas;

   public WaitingJoinUI waitingJoinUi;
   
   public List<PlanetUI> planetUis=new List<PlanetUI>();

   [Header("计时面板")]
   public Transform gameTimerTextBg;
   public Text gameTimerText;
   
   public void Init(GameManager gameManager)
   {
      this.gameManager = gameManager;
   }

   public void ResetUi()
   {
      waitingJoinUi.gameObject.SetActive(true);
      waitingJoinUi.ResetUi();
      gameTimerTextBg.gameObject.SetActive(false);
      StopAllCoroutines();
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

   public CommanderUI CreateCommanderUi(GameObject go, int area)
   {
      return hpCanvas.CreateCommanderUi(go,area);
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
   
   public GatherUI CreateGatherUi(Planet planet,Planet sponsor,PlanetCommander planetCommander)
   {
      var gatherUi=hpCanvas.CreateGatherUi();
      gatherUi.Init(planet,sponsor,planetCommander);
      return gatherUi;
   }

   public void OpenTimer()
   {
      gameTimerTextBg.gameObject.SetActive(true);
      StartCoroutine(UpdateTimerText());
   }

   IEnumerator UpdateTimerText()
   {
      int time = 0;
      while (true)
      {
         yield return new WaitForSeconds(1);
         time++;
         int minute = (int) time / 60;
         int second = (int) time % 60;
         gameTimerText.text = minute + ":" + second;
      }
   }
}
