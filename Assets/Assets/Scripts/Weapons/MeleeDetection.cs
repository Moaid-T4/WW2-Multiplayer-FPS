using UnityEngine;
using System.Collections;

public class MeleeDetection : MonoBehaviour {
    [SerializeField]
    Melee melee;

    void Start()
    {
        if (!melee)
            melee = transform.parent.GetComponent<Melee>();
    }

    public void Attacked()
    {
        melee.Damage();
    }
}
