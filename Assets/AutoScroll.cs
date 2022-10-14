using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    private ScrollRect scrollRect;

    public float speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        StartCoroutine(AutoScrollC());
    }

    IEnumerator AutoScrollC()
    {
        while (true)
        {
            //Debug.Log(scrollRect.verticalNormalizedPosition);
            if (scrollRect.verticalNormalizedPosition > 0.05f)
            {
                scrollRect.verticalNormalizedPosition -= speed * Time.deltaTime;
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(3);
                scrollRect.verticalNormalizedPosition = 1;
                yield return new WaitForSeconds(3);
            }
            
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
