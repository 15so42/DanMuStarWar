using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItemUi : MonoBehaviour
{
   public Image fill;
   public Text userNameText;
   public Text percentText;

   public void UpdateUi(float fillAmount,string userName,int percent)
   {
      fill.fillAmount = fillAmount;
      userNameText.text = userName;
      percentText.text = percent+"%";
   }
}
