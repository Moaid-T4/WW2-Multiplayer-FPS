using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    [SerializeField]
    int health;

    public int Health { get { return health; } }

    [SerializeField]
    Text healthText;

    FirstPersonController fps;

    void Start () {
        fps = GetComponent<FirstPersonController>();
        healthText.text = health + "%";
    }
	

	void Update () {
	    
	}

    public void TakeDamage(int damage)
    {
        print("damaged : " + damage);
        health -= damage;

        if (health <= 0)
        {
            health = 0;
        }

        healthText.text = health + "%";
    }
}
