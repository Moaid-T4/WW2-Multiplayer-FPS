using UnityEngine;
using System.Collections;

public class Prop : MonoBehaviour {
    [SerializeField]
    bool canDamage;
    [SerializeField]
    int health;

    [SerializeField]
    float hardness = 15;

    public bool CanDamage { get { return canDamage; } }
    public int Health { get { return health; } }

    public float Hardness { get { return hardness; } }

    void Start () {
	
	}
	
	void Update () {
	
	}

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            health = 0;
        }
    }
}
