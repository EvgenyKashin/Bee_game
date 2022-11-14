using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Player object
    public GameObject toFollow;

    // Camera offset
    public Vector3 offset = new Vector3(0, 1.6f, -3.3f);
    // Start is called before the first frame update

    void LateUpdate()
    {   
        // camera orientation that everytime in the back of the player (with rotation)
        Vector3 backOfToFollow = toFollow.transform.forward * offset.z +
            toFollow.transform.up * offset.y;
        
        // Interpolation with a camera that doesn't track a player rotation
        // up orientation
        float y_ = (backOfToFollow.y + 5f) / 5f;
        // back orientation
        float z_ = (backOfToFollow.z - toFollow.transform.forward.z * 5f) / 2.5f;

        // Other options
        // float y_ = backOfToFollow.y;
        // float y_ = offset.y;
        backOfToFollow = new Vector3(backOfToFollow.x, y_, z_);
        transform.position = toFollow.transform.position +
            // - toFollow.transform.forward * 2 + offset;
            backOfToFollow;
        transform.LookAt(toFollow.transform.position);

        // Fix camera moving under the ground
        if (transform.position.y < 0.5f) {
            transform.position = new Vector3(transform.position.x,
                0.5f, transform.position.z);
        }
    }
}
