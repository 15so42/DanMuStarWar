using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SteveAnim : MonoBehaviour
{

    private Transform root;
    private NavMeshAgent navMeshAgent;

    private Animator animator;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private BattleUnit battleUnit;

    public float divideRate = 6;

    // Start is called before the first frame update
    void Start()
    {
        root = transform.root;
        battleUnit = root.GetComponent<BattleUnit>();
        navMeshAgent = root.GetComponent<NavMeshAgent>();
        animator = battleUnit.animator;
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = 0f;
        velocity = navMeshAgent.velocity.magnitude;
        
        animator.SetFloat(Speed,velocity/divideRate);

    }
}
