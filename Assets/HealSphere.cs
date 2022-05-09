using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SkillContainer skillContainer = GetComponent<SkillContainer>();
        SkillManager.Instance.AddSkillToObj("Skill_治疗领域_LV1",skillContainer);
    }

  

   
}
