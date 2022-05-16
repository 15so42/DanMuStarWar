using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArea : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    void Start()
    {
        SkillContainer skillContainer = GetComponent<SkillContainer>();
        SkillManager.Instance.AddSkillToObj("Skill_点数领域_LV1",skillContainer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
