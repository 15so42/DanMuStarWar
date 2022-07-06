using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLightManager : MonoBehaviour
{
    public static DayLightManager Instance;

    public bool enable = true;
    private void Awake()
    {
        Instance = this;
    }

    //旋转直线光，0到180为从上往下照，180到360为从下网上，由于会漏光，因此从下网上照时隐藏光源
    public Light dayLight;
    
    [Header("整天时间")] public int wholeDaySeconds = 30;
    
    [Header("颜色")]
    public Color dayColor=new Color(1,1,1);
    public float dayIntensity = 1.7f;
    
    public Color nightColor=new Color((float)173/255,(float)208/255,(float)253/255);
    public float nightIntensity = 0.7f;

    public Color ambientNightColor=new Color((float)110/255,(float)185/255,1);

    public Light[] sceneLights;
    [Header("场景光强度")] public float daySceneLightIntensity = 2;
    public float nightSceneLightIntensity = 1;
    
    private float timer=0;

    private float lastTime = 0;
    private int day = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        dayLight.transform.localEulerAngles=new Vector3(1,-30,0);
        //dayLight.transform.forward=Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if(enable==false)
            return;
        timer += Time.deltaTime;
        dayLight.transform.Rotate(new Vector3((float)360/wholeDaySeconds*Time.deltaTime,0,0));
        

        var time = GetTime();
        //Debug.Log(time);
        var targetColor = dayColor;
        var targetIntensity = dayIntensity;
        var targetAmbientLight=new Color(0.85f,0.85f,0.85f);
        var targetSceneLightIntensity = daySceneLightIntensity;
        if (time > 9 && time < 19)
        {
            targetSceneLightIntensity = nightSceneLightIntensity;
        }
        else
        {
            targetColor = nightColor;
            targetIntensity = nightIntensity;
            targetAmbientLight = ambientNightColor;
            
        }
        dayLight.color = Color.Lerp(dayLight.color, targetColor, 0.5f * Time.deltaTime);
        dayLight.intensity = Mathf.Lerp(dayLight.intensity, targetIntensity, 0.5f * Time.deltaTime);
        RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, targetAmbientLight, 0.5f * Time.deltaTime);
        for (int i = 0; i < sceneLights.Length; i++)
        {
            sceneLights[i].intensity =
                Mathf.Lerp(sceneLights[i].intensity, targetSceneLightIntensity, 0.5f * Time.deltaTime);
        }

        //重置太阳角度和计时以免误差累计
        if ((int)time ==8 && (int)lastTime==7)
        {
            day++;
            dayLight.transform.localEulerAngles=new Vector3(1,-30,0);
            timer = 0;
            Debug.Log("新的一天，天数为"+day);
            

        }
        //Debug.Log(time+","+lastTime);
        lastTime = time;
        
    }

    public bool IsDay()
    {
        var time = GetTime();
        return time > 9 && time < 20;
    }

    float GetTime()
    {
        var wholeDay = wholeDaySeconds * 2;
        return (8+((timer % wholeDay) / wholeDaySeconds) * 24)%24;
    }
}
