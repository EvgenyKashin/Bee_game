using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Player object
    public GameObject toFollow;

    // Camera offset
    public Vector3 offset = new Vector3(0, 0.7f, -3f);

    public bool isDynamicCamera = true;

    void LateUpdate()
    {   
        // camera orientation that everytime in the back of the player (with rotation)
        Vector3 backOfToFollow = toFollow.transform.forward * offset.z +
            toFollow.transform.up * offset.y;
        // Save up scaler of the vector
        float staticY = backOfToFollow.y;

        // Dynamic camera rotates up and down a little
        if (isDynamicCamera) {
            // set up scaler to 0 and normalize
            backOfToFollow = new Vector3(backOfToFollow.x, 0, backOfToFollow.z);
            backOfToFollow = -Vector3.Normalize(backOfToFollow) * offset.z;
            // return up scaler back and average it with straight up orientation
            backOfToFollow = backOfToFollow + Vector3.up * (offset.y + staticY) / 2;
        }

        transform.position = toFollow.transform.position +
            backOfToFollow;
        transform.LookAt(toFollow.transform.position);
                        //  + toFollow.transform.up * 0.5f);

        // Fix camera moving under the ground
        if (transform.position.y < 0.5f) {
            transform.position = new Vector3(transform.position.x,
                0.5f, transform.position.z);
        }
    }

    public void CameraToggle(bool tog) {
        isDynamicCamera = tog;
    }
}
