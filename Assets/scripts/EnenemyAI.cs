using JetBrains.Annotations;
using System.Diagnostics;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnenemyAI : MonoBehaviour
{
    public NavMeshAgent Agent;

    public Transform Player;

    public LayerMask WhatIsGround, WhatIsPlayer;

    public float health;

    //patrolling

    public Vector3 Walkpoint;
    bool walkPointSet;
    bool Walkset; //Controleren wanneer de Enemy wel of niet moet lopen//
    public float WalkpointRange; //Om de aftsand te controleren wanneer de Enemy moet lopen//

    //Attack
    public float TimeBewtweenAttacks;
    bool AlreadyAttacked; //controleren of de Enemey heeft geattacked//
    public GameObject projectile;//maakt een verwijzing naar een prefab van een projectiel (zoals een kogel, vuurbal(kan je zien in Unity inspector)

    //States 

    public float sightRange, attackingRange; //sightRange: is voor de maximale afstand wat de speler kan zien)AttackingRange:De afstand wanneer de Enemy begint aan te vallen)

    public bool PlayerInSightRange, PlayerInAttackRange; // om te kijken dat de speler binnen de SightRange & PlayerInAttackRange zit.

    private void Awake()
    {
        Player = GameObject.Find("EnemyOBJ").transform; //Het zoekt een object met de naam "EnemyOBJ" en bewaart de plek ervan, zodat de vijand weet waar de speler is en wat hij moet doen.
        Agent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        //Check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer); //controleert of de speler binnen het zichtbereik van de vijand is door een onzichtbare bol te tekenen rond de vijand.

        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackingRange, WhatIsPlayer);//controleert of de speler dicht genoeg bij is om aan te vallen.

        if (!PlayerInSightRange && !PlayerInAttackRange) Patroling();//Als de speler niet te zien is én niet dichtbij genoeg is om aan te vallen, dan moet de vijand gaan patrouilleren.

        if (PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();//Als de vijand de speler ziet maar hij is nog te ver weg, gaat hij achter de speler aan.

        if (PlayerInAttackRange && PlayerInSightRange) AttackPlayer();//De vijand valt aan als hij de speler ziet en deze dichtbij genoeg is.


    }
     private void Patroling() 
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) 
        Agent.SetDestination(Walkpoint); // als de looproute is gekozen gaat de Enemy erheen

        Vector3 distanceToWalkPoint = transform.position - Walkpoint; //berekent de afstand tussen de vijand en het (Walkpoint).

        //Walkpoint reached

        if (distanceToWalkPoint.magnitude <1f)//Als de afstand kleiner is dan 1 meter, dan is het punt bereikt.(Magnitude: is hoelang is de Vector (Afstand)
            walkPointSet = false;//Dus: tijd om een nieuw doelpunt te kiezen.
    }
    void SearchWalkPoint()
    {
        //caculate random point in range
        float randomZ = Random.Range(-WalkpointRange, WalkpointRange); //Het kiest een willekeurige afstand vooruit of achteruit, binnen een bepaald bereik hiermee bepaald de Enemy zelf waar hij patroueerd.

        float randomX = Random.Range(-WalkpointRange, WalkpointRange); // Dan kies je een willekeurig punt rondom de vijand, zodat hij een beetje rondloopt in de buurt.


        Walkpoint = new Vector3  (transform.position.x + randomX, transform.position.y, transform.position.z + randomZ); //wordt gebruikt om een willekeurig punt te kiezen in de buurt van de vijand, waar hij naartoe kan lopen tijdens het patrouilleren.

        if (Physics.Raycast (Walkpoint, -transform.up, 2f, WhatIsGround)) //Deze regel controleert of het gekozen loopdoelpunt(Walkpoint) op de grond ligt.
            //Raycast: onzichtbarere straal die start van het WalkPoint 2 meter  (-transform.up betekent "omlaag")  Kijkt of het iets raakt op de laag WhatIsGround (dat is meestal de vloer)

            walkPointSet = true; //Als het true is kan de vijand hiernaar toe lopen.
    }
    private void ChasePlayer() 
    { 
    Agent.SetDestination (Player.position); //zorgt ervoor dat de vijand naar de speler toe loopt.


    }
           
    private void AttackPlayer()
    {
        //make sure enemy doesn't move

        Agent.SetDestination(transform.position); //"Stop met lopen

        transform.LookAt(Player);//Draai naar de speler toe


        if (!AlreadyAttacked) // Als de vijand nog niet heeft aangevallen..., dan gaat hij nu aanvallen.


        {

            //Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();  //Quaternion.identity: betekent: zonder draaiing (recht vooruit).

            ///Vraag aan docent wat dit doet (ERWIN Henraat)
            rb.AddForce(transform.forward *32f, ForceMode.Impulse); // Geef het projectiel een duw vooruit en een beetje omhoog, zodat het "wegvliegt" richting de speler.
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);


            //Cooldown
            AlreadyAttacked = true;//Zet AlreadyAttacked op true, zodat hij niet meteen nog een keer kan schieten.

            Invoke(nameof(ResetAttack),TimeBewtweenAttacks);//Wacht een tijdje (bijv. 1 seconde), en dan wordt ResetAttack() automatisch aangeroepen.


        }
    }

    private void ResetAttack()
    {
        AlreadyAttacked = false; //Het zet de aanval van de vijand weer terug naar false, zodat hij opnieuw mag aanvallen.
    }

    public void TakeDamage(int damage)
    {
        health -= damage;//trekt je aantal health van de gezondheid.



        if (health <= 0) Invoke(nameof(DestroyEnemy), 5f);//Als de gezondheid op of onder nul komt, start je een timer van 5 seconden
    }
    private void DestroyEnemy()
    {
    Destroy(gameObject);//Wordt de vijand uit het spel verwijderd.

    }
    void Start(){ }


}





