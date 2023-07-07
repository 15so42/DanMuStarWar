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
   
   public List<PlayerIconUi> playerIconUis=new List<PlayerIconUi>();
   
   private int countDownMaxTime = 120;
   public void Awake()
   {
      EventCenter.AddListener<Player>(EnumEventType.OnPlayerJoined,OnPlayerJoined);
      EventCenter.AddListener(EnumEventType.OnBattleStart,OnBattleStarted);
   }

   public void ResetUi()
   {
      var maxPlayer = FightingManager.Instance.maxPlayerCount;
      playerNumText.text = "等待玩家加入中[0"+"/"+maxPlayer+"]";
      var children = playerGroupTrans.GetComponentsInChildren<Transform>();
      for (int i = 1; i < children.Length; i++)
      {
         Destroy(children[i].gameObject);
      }
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
      var maxPlayer = FightingManager.Instance.maxPlayerCount;
      playerNumText.text = "等待玩家加入中["+GameManager.Instance.fightingManager.players.Count+"/"+maxPlayer+"]";
      var icon = GameObject.Instantiate(playerIconPfb, playerGroupTrans);
      var iconUi=icon.GetComponent<PlayerIconUi>();
      iconUi.Init(player);
      playerIconUis.Add(iconUi);
   }

   public PlayerIconUi GetPlayerIconUiByUid(long uid)
   {
      return playerIconUis.Find(x => x.player.uid == uid);
   }

   private void OnDestroy()
   {
      EventCenter.RemoveListener<Player>(EnumEventType.OnPlayerJoined,OnPlayerJoined);
      EventCenter.RemoveListener(EnumEventType.OnBattleStart,OnBattleStarted);
   }
}
