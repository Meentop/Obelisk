using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Cycles cycles;

    [SerializeField] float baseSpeed;

    private void Start()
    {
        cycles = Cycles.Instance;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = FindObjectOfType<EmpirePortal>().transform.position;
    }

    private void Update()
    {
        agent.speed = baseSpeed * cycles.timeScale;
    }
}
