using UnityEngine;
using System.Collections;

public class LootAt : MonoBehaviour {

    Transform cam;

	void Start () {
        cam = Camera.main.transform;
	}


	void Update () {
        transform.LookAt(cam);
	}
}
