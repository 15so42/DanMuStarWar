using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TipsText : MonoBehaviour
{
    public Text textComp;

    public string[] tips;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RandomTips());
    }

    

    IEnumerator RandomTips()
    {
        while (true)
        {
            var position = textComp.transform.localPosition;
            textComp.transform.DOLocalMove(position + Vector3.right * 1200, 1);
            
            yield return new WaitForSeconds(3);
            
            textComp.text = tips[UnityEngine.Random.Range(0, tips.Length)];
            textComp.transform.DOLocalMove(textComp.transform.localPosition - Vector3.right * 1200, 1);
            yield return new WaitForSeconds(10);
        }
    }
}
