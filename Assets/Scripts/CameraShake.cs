using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake m_this = null;
    public static CameraShake Instance
    {
        get
        {
            return m_this;
        }
    }

    void Awake()
    {
        m_this = this;
    }

    public float effectTime;
    private bool startShake = false;
    private float seconds = 0f;
    public AnimationCurve shakeQuake;

    public void Play()
    {
        seconds = 0;
        startShake = true;
    }

    //private void Update()
    //{
    //       if (Input.GetKeyDown(KeyCode.A))
    //       {
    //           seconds = 0;
    //           startShake = true;
    //       }
    //}

    void LateUpdate()
    {
        if (!startShake)
        {
            return;
        }

        seconds += Time.deltaTime;

        float nPct = seconds / effectTime;
        transform.localPosition = Random.insideUnitSphere * shakeQuake.Evaluate(nPct);

        if (nPct >= 1)
        {
            transform.localPosition = Vector3.zero;
            startShake = false;
        }
    }
}
