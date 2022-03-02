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
   public Action onFinished;

   public TaskParams(TaskType taskType, string value, float duration, Action onFinished = null)
   {
      this.taskType = taskType;
      this.value = value;
      this.duration = duration;
      this.onFinished = onFinished;
   }
}
public class PlanetTask : MonoBehaviour
{
   private float timer = 0;
   private bool isFininshed = false;
   
   public TaskParams taskParams;
   
   public PlanetTask(TaskParams taskParams)
   {
      this.taskParams = taskParams;
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

   void OnFinished()
   {
      switch (taskParams.taskType)
      {
         case TaskType.Create :
            var splitArr = taskParams.value.Split('_');
            var type = splitArr[0];

            if (type == "Planet")
               ResFactory.Instance.CreatePlanet(splitArr[1]);
            if (type == "BattleUnit")
               ResFactory.Instance.CreateBattleUnit(splitArr[1]);
            if (type == "Bullet")
               ResFactory.Instance.CreateBullet(splitArr[1]);
            break;
      }
   }
   
}
