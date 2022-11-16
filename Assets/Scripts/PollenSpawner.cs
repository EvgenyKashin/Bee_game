using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenSpawner : MonoBehaviour
{
   public GameObject pollenPrefab;
   private float startDelay = 5f;
   private float targetTime;
   private int spawnedAlready = 0;

   void Start() 
   {
		  targetTime = startDelay;
   }
   
   void FixedUpdate()
   {
		  targetTime -= Time.deltaTime;
		  if (targetTime < 0) {
				spawnedAlready += 1;
				Instantiate(pollenPrefab, transform.position + new Vector3(Random.Range(0, 0.5f), Random.Range(0, 0.5f), Random.Range(0, 0.5f)), Quaternion.identity);
				targetTime = Random.Range(5 * spawnedAlready, 10 * spawnedAlready);
		  }
   }
}
