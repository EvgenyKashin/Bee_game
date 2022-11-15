using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollenSpawner : MonoBehaviour
{
   public GameObject pollenPrefab;

   void Update()
   {
		  if (Input.GetKeyDown(KeyCode.DownArrow))
		  {
				Instantiate(pollenPrefab, transform.position, Quaternion.identity);
		  }
   }
}
