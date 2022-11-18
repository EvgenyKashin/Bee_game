using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PollenPickup : MonoBehaviour
{
    private float timeAlive;
    public float timeToLive = 5 * 60f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            PlayerControl playerControl = other.gameObject.GetComponent<PlayerControl>();
            Destroy(gameObject);
            playerControl.IncreaseScore();
            playerControl.PlayPollenCollection();
        }
    }

    void FixedUpdate() 
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > timeToLive) {
            Destroy(gameObject);
        }
    }
}
