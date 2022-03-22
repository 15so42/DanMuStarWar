
using UnityEngine;
using UnityEngine.UI;
 
public class FpsUI : MonoBehaviour
{
    public Text FpsText;
    private float time;
    private int frameCount;
    
    
    void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= 1 && frameCount >= 1)
        {
            float fps = frameCount / time;
            time = 0;
            frameCount = 0;
            FpsText.text = fps.ToString("f2");//#0.00
            FpsText.color = fps >= 20 ? Color.white : (fps > 15 ? Color.yellow : Color.red);
        }
    }
}
 
