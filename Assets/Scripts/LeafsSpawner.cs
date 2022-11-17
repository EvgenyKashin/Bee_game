using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafsSpawner : MonoBehaviour
{
    public int spawnEveryNUpdates = 10;
    public GameObject spawningObject;
    private MeshCollider meshCol;
    private int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        meshCol = gameObject.GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        counter += 1;
        if (counter % spawnEveryNUpdates == 0) 
        {
            Vector3 currentPos = transform.position;
            Vector3 currentScale = transform.localScale;
            Vector3 randomPos;

            float x = currentPos.x + Random.Range( -meshCol.bounds.size.x / 2, meshCol.bounds.size.x / 2 );
            float z = currentPos.z + Random.Range( -meshCol.bounds.size.z / 2, meshCol.bounds.size.z / 2 );

            randomPos = currentPos + new Vector3(x, 0, z);
            Debug.Log(randomPos);

            GameObject go = Instantiate(spawningObject, randomPos, transform.rotation);
            go.transform.rotation = Random.rotation;
        }
    }
}
