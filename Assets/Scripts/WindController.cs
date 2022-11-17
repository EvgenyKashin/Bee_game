using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public float ChangeEveryNSec = 5f;
    private float timeFromLastChange = 0f;
    // Update is called once per frame
    void FixedUpdate()
    {
        timeFromLastChange += Time.deltaTime;
        if (timeFromLastChange > ChangeEveryNSec) {
            timeFromLastChange = 0;
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            transform.rotation = randomRotation;
        }
    }
}
