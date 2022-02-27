﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   private GameManager gameManager;
   public void Init(GameManager gameManager)
   {
      this.gameManager = gameManager;
   }

   public void ResetUi()
   {
      Debug.Log("ResetUI待实现");
   }

   public void OnPlayerJoined(Player player)
   {
      Debug.Log(nameof(OnPlayerJoined)+"待实现");
   }

   public AccountUI GetAccountUiByPlayer(Player player)
   {
      return null;
   }
}
