using System;
using System.Collections;
using System.Collections.Generic;
using GameCode.Tools;
using UnityEngine;

public enum TaskType
{
   Create,
   Upgrade,
}

public struct TaskParams
{
   public TaskType taskType;
   
   public string value;
   public float duration;
   public Action<GameObject> onFinished;

   public TaskParams(TaskType taskType, string value, float duration, Action<GameObject> onFinished = null)
   {
      this.taskType = taskType;
      
      this.value = value;
      this.duration = duration;
      this.onFinished = onFinished;
   }
}


public class PlanetTask 
{
   private float timer = 0;
   public PlanetCommander planetCommander;
   public bool isFininshed = false;

   public TaskParams taskParams;
   public ITaskAble owner;
   
   public PlanetTask(TaskParams taskParams,PlanetCommander planetCommander)
   {
      this.taskParams = taskParams;
      this.planetCommander = planetCommander;
      //init
      timer = taskParams.duration;
   }
   public void Run()
   {
      
      if (!isFininshed )
      {
         timer -= Time.deltaTime;
         if (timer <= 0)
         {
            isFininshed = true;
            OnFinished();
         }
      }
   }

   public void InitBattleUnit(BattleUnit battleUnit,PlanetCommander planetCommander)
   {
      battleUnit.Init(owner as Planet,planetCommander);
      
   }

   void OnFinished()
   {
      switch (taskParams.taskType)
      {
         case TaskType.Create :
            var splitArr = taskParams.value.Split('_');
            var type = splitArr[0];

            GameObject go = null;
            if (type == "Planet")
            {
               go = ResFactory.Instance.CreatePlanet(splitArr[1]);
            }

            if (type == "BattleUnit")
            {

               var pos = owner.GetSpawnPoint().position;
               if (FightingManager.Instance.gameMode == GameMode.MCWar)
                  pos = owner.GetGameObject().transform.position;
//               Debug.Log(pos);
               go=ResFactory.Instance.CreateBattleUnit(splitArr[1],pos);
               
               go.name = go.name + owner.GetGameObject().name;
               InitBattleUnit(go.GetComponent<BattleUnit>(),planetCommander);
               taskParams.onFinished?.Invoke(go);
            }

            if (type == "Bullet")
            {
               go=ResFactory.Instance.CreateBullet(splitArr[1],Vector3.zero);
            }
               
            
            
            break;
      }
      
   }
   
}
