using System.Collections;
using System.Collections.Generic;
using Ludiq;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    

    public Transform[] clouds;

    public Vector3 cloudSize = Vector3.one;

    private Transform iCloud = null;
    // Start is called before the first frame update
    void Start()
    {
        var parent=new GameObject("CloudParent");
        
        parent.transform.SetParent(transform);
        parent.transform.localPosition=Vector3.zero;
        parent.transform.localScale = Vector3.one;
        
        iCloud = GameObject.Instantiate(clouds[Random.Range(0,clouds.Length)],parent.transform);
        var transform1 = iCloud.transform;
        transform1.localPosition=Vector3.zero;
        transform1.localScale=cloudSize;
        iCloud.gameObject.AddComponent<RotateSelf>();

    }

    public void Close()
    {
        //关闭生成的云
        if(iCloud)
            iCloud.gameObject.SetActive(false);
        this.enabled = false;
    }

    
}
