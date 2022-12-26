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

   public Text mapVoteText;
   
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
      //隐藏投票UI
      mapVoteText.text = "";
   }

   public void UpdateWaitingJoinUI(float time)
   {
      waitingJoinUi.UpdateUI(time);
   }

   public void UpdateMapVoteUi(int normalCounter,int bgCounter)
   {
      mapVoteText.text = "模式投票-混战/团战:" + normalCounter + "/" + bgCounter;
   }
   

   public PlanetUI GetPlanetUiByPlayer(Player player)
   {
      return planetUis.Find(x => x.player == player);
   }

   public SummonPointUi CreateSummonPointUi(GameEntity gameEntity)
   {
      return hpCanvas.CreateSummonPointUi(gameEntity);
   }
   
   public HpBar CreateHpBar(GameEntity gameEntity)
   {
      return hpCanvas.CreateHpBar(gameEntity);
   }

   public CommanderUI CreateCommanderUi(GameObject go, int area)
   {
      return hpCanvas.CreateCommanderUi(go,area);
   }
   
   
   public CommanderUI CreateSteveCommanderUi(GameObject go, int area)
   {
      return hpCanvas.CreateSteveCommanderUi(go,area);
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

   public McPosMarkUi CreateMcPosMarkUi()
   {
      var ui = hpCanvas.CreateMcMarkUi();
      return ui;
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
