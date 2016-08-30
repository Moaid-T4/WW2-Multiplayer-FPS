using UnityEngine;
using System.Collections;

public class OverLapPass : MonoBehaviour {
    public GameObject reciever;

    void OnTriggerEnter(Collider col)
    {
        reciever.SendMessage("TriggerEnter", col);
    }

    void OnTriggerExit(Collider col)
    {
        reciever.SendMessage("TriggerExit", col);
    }
}
