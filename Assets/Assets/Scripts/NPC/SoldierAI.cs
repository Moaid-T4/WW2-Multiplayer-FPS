using UnityEngine;
using System.Collections;

public class SoldierAI : MonoBehaviour {

    [SerializeField]
    int health;

    public int Health { get { return health; } }

    [SerializeField]
    Transform player;

    [SerializeField]
    bool agro;

    [SerializeField]
    float checkDistance;

    [SerializeField]
    LayerMask mask;

    Animator anim;
    NavMeshAgent agent;

    void Start ()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            health = 0;
            Die();
        }

    }

    public void Die()
    {

    }

    public void TriggerEnter(Collider col)
    {
        print(col.name + " has entered");
    }

    public void TriggerExit(Collider col)
    {
        print(col.name + " has exited");
    }
}
