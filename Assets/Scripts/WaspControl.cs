using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspControl : MonoBehaviour
{
    public GameObject player;
    private PlayerControl playerControl;
    private Rigidbody rb;
    private Material material;
    public Vector2 patrolReturnPoint = new Vector2(0, 15);
    public float patrolRadius = 5;
    public float patrolHeight = 3;
    public float minHeight = 1f;

    public float maxHeight = 5;
    public float visionRadius = 10;
    public Vector2 nextPoint;
    private Vector2 pointToMove;
    private float yToMove;

    public float verticalForce = 2;
    public float movementForce = 0.2f;
    public float rotationForce = 0.3f;
    public float damagePerTouch = 0.3f;

    private float patrolThreshold = 0.5f;
    public int updateEachNFrame = 5;
    private int frameUpdated = 0;
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("player")[0];
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 1.5f;

        playerControl = player.GetComponent<PlayerControl>();
        material = GetComponent<Renderer>().material;

        patrolReturnPoint = new Vector2(transform.position.x, transform.position.z);
        transform.position = new Vector3(patrolReturnPoint.x,
            patrolHeight, patrolReturnPoint.y);
        nextPoint = patrolReturnPoint;
    }

    Vector2 getPosition2d() 
    {
        return new Vector2(transform.position.x, transform.position.z);
    }

    Vector3 get3dfrom2d(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    Vector2 get2dfrom3d(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    float getDistance2d(Vector2 point) 
    {
        Vector2 position2d = getPosition2d();
        return Vector2.Distance(position2d, point);
    }
    
    Vector2 getNextPoint() 
    {   
        // Overwise, patrol to the next point
        float distance = getDistance2d(nextPoint);
        if (distance < patrolThreshold) {
            // Sample a new point
            float randomAngle = Random.Range(0, Mathf.PI * 2);
            float x = Mathf.Cos(randomAngle);
            float y = Mathf.Sin(randomAngle);
            nextPoint = patrolReturnPoint + new Vector2(x, y) * patrolRadius;
        }
        return nextPoint;
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        if (frameUpdated % updateEachNFrame != 0) {
            float playerDistance = Vector3.Distance(transform.position,
                player.transform.position);
            
            // Follow player if they close
            if (playerDistance < visionRadius) {
                pointToMove = get2dfrom3d(player.transform.position);
                yToMove = Mathf.Min(player.transform.position.y, maxHeight);
                material.color = Color.red;
            } else {
                pointToMove = getNextPoint();
                yToMove = patrolHeight + Random.Range(-0.2f, 0.2f);
                material.color = Color.gray;
            }
            yToMove = Mathf.Max(yToMove, minHeight);

            Vector2 direction = (pointToMove - getPosition2d()).normalized;
            rb.AddForce(get3dfrom2d(direction) * movementForce);
            
            if (transform.position.y < yToMove) {
                rb.AddForce(Vector3.up * verticalForce);
            }
            Quaternion rotation = Quaternion.LookRotation(get3dfrom2d(direction));
            rotation = transform.rotation * Quaternion.Inverse(rotation);
            rb.AddTorque(new Vector3(rotation.x, rotation.y, rotation.z) * rotation.w * rotationForce);
        }

        // // Air drag force to make flying slower
        // Vector3 dragDirection = -rb.velocity.normalized;
        // float velocityMag = rb.velocity.magnitude;
        // rb.AddForce(velocityMag * velocityMag * dragDirection 
        //     * playerControl.airResistance);
        
        Vector3 predictedUp = Quaternion.AngleAxis(
            rb.angularVelocity.magnitude * Mathf.Rad2Deg * playerControl.stabilityForce / playerControl.stabilitySpeed,
            rb.angularVelocity
        ) * (-transform.forward);
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rb.AddTorque(torqueVector * playerControl.stabilitySpeed * playerControl.stabilitySpeed);

        frameUpdated += 1;
    }
}
