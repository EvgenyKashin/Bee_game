using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafController : MonoBehaviour
{
    public float timeToDestroyAfterGround = 1f;
    public float timeToDestroyAny = 5f;
    private bool inWindZone = false;
    private GameObject windZone;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // Wind force
        if (inWindZone) {
            rb.AddForce(windZone.transform.forward *
                windZone.GetComponent<WindZone>().windMain * 2);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "windZone") {
            windZone = coll.gameObject;
            inWindZone = true;
        }
    }

    void OnTriggerExit(Collider coll) 
    {   
        if (coll.gameObject.tag == "windZone") {
            inWindZone = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "ground") {
            Invoke("selfDestroy", timeToDestroyAfterGround);
        } else if (col.gameObject.tag != "windZone" &&
                   col.gameObject.tag != "leafSpawner") {
            // to ingore windZone and spawner itself
            Invoke("selfDestroy", timeToDestroyAny);
        }
    }

    void selfDestroy()
    {
        Destroy(gameObject);
    }
}
