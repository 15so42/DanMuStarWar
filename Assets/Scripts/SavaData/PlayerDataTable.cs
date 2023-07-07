using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDataTable
{
   public List<PlayerData> playerDataList=new List<PlayerData>();

   public PlayerData FindByUid(long uid)
   {
      var playerData = playerDataList.Find(x => x.uid == uid);
      if (playerData == null)
      {
         playerData=new PlayerData()
         {
            uid=uid,
         };
         playerDataList.Add(playerData);
      }
      return playerData;
   }

   public void UpdateByUid(long uid,PlayerData newData)
   {
      var playerData = playerDataList.Find(x => x.uid == uid);
      if (playerData!=null)
      {
         //playerData.userName = newData.userName;
         playerData.giftPoint = newData.giftPoint;
         playerData.opendScore = newData.opendScore;
         playerData.winCount = newData.winCount;
         playerData.loseCount = newData.loseCount;
      }
      else
      {
         playerDataList.Add(newData);
      }
      
   }
}
