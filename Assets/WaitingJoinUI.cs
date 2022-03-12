using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingJoinUI : MonoBehaviour
{
   public TMP_Text playerNumText;
   public TMP_Text countDownText;

   public GameObject playerIconPfb;
   public Transform playerGroupTrans;
   
   private int countDownMaxTime = 120;
   public void Awake()
   {
      EventCenter.AddListener<Player>(EnumEventType.OnPlayerJoined,OnPlayerJoined);
      EventCenter.AddListener(EnumEventType.OnBattleStart,OnBattleStarted);
   }

   private void Start()
   {
      countDownMaxTime = GameManager.Instance.fightingManager.waitingJoinSecond;
   }

   public void UpdateUI(float time)
   {
      countDownText.text = (int)(countDownMaxTime - time)+"秒后开始游戏";
   }

   void OnBattleStarted()
   {
      gameObject.SetActive(false);
   }

   public void OnPlayerJoined(Player player)
   {
      var maxPlayer = PlanetManager.Instance.ownerAblePlanets.Count;
      playerNumText.text = "等待玩家加入中["+GameManager.Instance.fightingManager.players.Count+"/"+maxPlayer+"]";
      var icon = GameObject.Instantiate(playerIconPfb, playerGroupTrans);
      icon.GetComponent<PlayerIconUi>().Init(player);
   }
}
