using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public float floatSpeed;
    public float floatTimer;
    // Update is called once per frame
    void Update()
    {
        floatTimer += Time.deltaTime; 
        Vector3 moveDir = new Vector3 (0.0f, floatSpeed, 0.0f);
        transform.Translate(moveDir);

    }
}
