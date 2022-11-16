using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PollenPickup : MonoBehaviour
{
    private PlayerControl playerControl;

    void Start()
    {
        playerControl = GameObject.FindGameObjectsWithTag("player")[0].GetComponent<PlayerControl>();
    }


    void OnTriggerEnter(Collider other)
        {
 
            if (other.gameObject.tag == "player")
           {
                    Destroy (gameObject);
                    playerControl.IncreaseScore(); 
           }
        }
}
