using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class EnenemyAI : MonoBehaviour
{
    public NavMeshAgent Agent;

    public Transform Player;

    public LayerMask WhatIsGround, WhatIsPlayer;

    //patrolling

    public Vector3 Walkpoint;
    bool Walkset; //Controleren wanneer de Enemy wel of niet moet lopen//
    public float WalkpointRange; //Om de aftsand te controleren wanneer de Enemy moet lopen//

    //Attack
    public float TimeBewtweenAttack;

    void Start()
    {
   
    }


    void Update()
    {
        
    }
}
