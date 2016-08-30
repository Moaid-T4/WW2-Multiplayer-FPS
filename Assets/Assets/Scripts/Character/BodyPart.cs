using UnityEngine;
using System.Collections;

public class BodyPart : MonoBehaviour {
    [SerializeField]
    GameObject character;

    [SerializeField]
    float hardness = 15;

    public GameObject Character { get { return character; } }

    public float Hardness { get { return hardness; } }

    void Start () {
	    
	}
	
	void Update () {
	    
	}
}
