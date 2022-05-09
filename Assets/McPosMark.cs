using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McPosMark : MonoBehaviour
{
    private McPosMarkUi markUi;
    // Start is called before the first frame update
    void Start()
    {
        markUi = FightingManager.Instance.uiManager.CreateMcPosMarkUi();
        markUi.Init(gameObject,transform.GetSiblingIndex());
    }

   
}
