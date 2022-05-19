using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyTextUi : MonoBehaviour
{
    private Text text;
    
    void Awake()
    {
        text = GetComponent<Text>();
    }
    public void Init(string msg, Color color)
    {
        text.text = msg;
        text.color = color;
    }
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
