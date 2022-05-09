using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class McPosMarkUi : MonoBehaviour
{
    public Text text;

    public  Vector3 offset=new  Vector3(0,25,0);
    public GameObject obj;

    private Camera mainCamera;

    private int index;
    // Start is called before the first frame update

    public void Init(GameObject obj, int index)
    {
        this.obj = obj;
        this.index = index;
    }
    void Start()
    {
        mainCamera=Camera.main;
        this.text.text = "> " + index + " <";
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCamera.WorldToScreenPoint(obj.transform.position) + offset;
    }
}
