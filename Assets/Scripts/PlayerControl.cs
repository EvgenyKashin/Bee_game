using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{   
    public enum DifficultyLevel {
        Easy,
        Hard
    }

    // Public game's settings
    //   Player's controls
    public float verticalForseMultiplier = 2;
    public float horizontalForseMultiplier = 0.04f;
    public float mouseXMultiplier = 0.05f;
    public float mouseYMultiplier = 0.05f;
    public float mouseYFrontMultiplier = 1f;
    public float mouseNeutralZone = 0.1f;
    //   Forces' controls
    public float airResistance = 0.15f;
    public float airUpForce = 0.5f;
    public float stabilityForce = 0.1f;
    public float stabilitySpeed = 0.3f;
    //   Restrictions' controls
    public float maximumXRotation = 0.3f;
    public float xRotationDecay = 1.05f;
    //   Health Points controls
    public float maxHP = 100;

    // Components
    private GameObject windZone;
    private Rigidbody rb;
    private HealthBar healthBar;

    // Binary flags
    private bool inWindZone = false;
    private bool isTouchingGround = false;
    private bool isControlled = false;

    // Enums
    public DifficultyLevel difficulty = DifficultyLevel.Easy;

    // Internal variables
    private float verticalInput;
    private float horizontalInput;
    private float mouseXValue;
    private float mouseYValue;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 2;

        healthBar = GameObject.FindGameObjectsWithTag("healthBar")[0].GetComponent<HealthBar>();
        healthBar.HealthPointsMax = maxHP;
        healthBar.HealthPoints = maxHP;

    }

    void FixedUpdate()
    {
        isControlled = false;

        // Inputs reading
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        mouseXValue = Mathf.Clamp(Input.mousePosition.x / Screen.width, 0, 1) - 0.5f;
        mouseYValue = Mathf.Clamp(Input.mousePosition.y / Screen.height, 0, 1) - 0.5f;

        // Vertical input
        if (Mathf.Abs(verticalInput) > 0.01f) {
            rb.AddForce(transform.up * verticalInput * verticalForseMultiplier);
            isControlled = true;
        }

        // Horizontal input
        if (Mathf.Abs(horizontalInput) > 0.01f) {
            // OR rb.AddRelativeForce(Vector3.forward..)
            rb.AddTorque(-transform.forward * horizontalInput * horizontalForseMultiplier);
            isControlled = true;
        } else {
            if (difficulty == DifficultyLevel.Easy) {
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y, rb.angularVelocity.z / 1.05f);
            }
        }

        // Mouse vertical input
        if (Mathf.Abs(mouseYValue) > mouseNeutralZone) {
            rb.AddTorque(transform.right * mouseYValue * mouseYMultiplier);
            if (mouseYValue > 0 && !isTouchingGround) {
                // Force to move in forward direction when rotate forward
                // Change to global forward?
                rb.AddForce(transform.forward * mouseYValue * mouseYFrontMultiplier);
            }
            isControlled = true;
        } else {
            if (difficulty == DifficultyLevel.Easy) {
                rb.angularVelocity = new Vector3(rb.angularVelocity.x / 1.05f, rb.angularVelocity.y, rb.angularVelocity.z);
            }
        }

        // Mouse horizontal input
        if (Mathf.Abs(mouseXValue) > mouseNeutralZone) {
            rb.AddTorque(transform.up * mouseXValue * mouseXMultiplier);
            isControlled = true;
        } else {
            if (difficulty == DifficultyLevel.Easy) {
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y / 1.03f, rb.angularVelocity.z);
            }
        }

        // Forward rotation restriction 
        if (((transform.rotation.x > maximumXRotation) 
            || (transform.rotation.x < -maximumXRotation))
            && difficulty == DifficultyLevel.Easy) {
            // Another option - strictly limit rotation:
            // transform.rotation = new Quaternion(
            //     // just to avoid to "if"s for > 0 and < 0
            //     maximumXRotation * (transform.rotation.x > 0 ? 1 : -1),
            //     transform.rotation.y, transform.rotation.z, transform.rotation.w);

            // Limit rotation velocity after maximumXRotation is achieved
            rb.angularVelocity = new Vector3(rb.angularVelocity.x / xRotationDecay, rb.angularVelocity.y, rb.angularVelocity.z);
        }

        // Air drag force to make flying slower
        Vector3 dragDirection = -rb.velocity.normalized;
        float velocityMag = rb.velocity.magnitude;
        rb.AddForce(velocityMag * velocityMag * dragDirection * airResistance);

        // Some small constant force to the UP direction
        rb.AddForce(Vector3.up * airUpForce);

        // Helping stabilization when there are no inputs control
        if (!isControlled && difficulty == DifficultyLevel.Easy) {
            Vector3 predictedUp = Quaternion.AngleAxis(
                rb.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityForce / stabilitySpeed,
                rb.angularVelocity
            ) * transform.up;
            Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
            rb.AddTorque(torqueVector * stabilitySpeed * stabilitySpeed);
        }

        // Reset rotation position when on the ground
        if (Input.GetKey(KeyCode.Space) && isTouchingGround) {
            // Reset except for horizontal rotation
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        // Wind force
        if (inWindZone) {
            rb.AddForce(windZone.transform.forward *
                windZone.GetComponent<WindZone>().windMain);
        }

        // It would be updated by event before each "Update" call
        isTouchingGround = false;
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

    void OnCollisionStay(Collision coll) 
    {       
        // For reseting by "Space" key
        if (coll.gameObject.tag == "ground") {
            isTouchingGround = true;
        }
        
        // Damage from enemies 
        // Called every physics update, aka FixedUpdate
        if (coll.gameObject.tag == "enemy") {
            if (coll.collider.gameObject.tag == "enemyDamager") {
                WaspControl waspControl = coll.gameObject.GetComponent<WaspControl>();
                healthBar.HealthPoints -= waspControl.damagePerTouch;
            } else {
                // Do some VFX
            }
        }
    }

    public void DifficultyToggle(bool tog) {
        difficulty = tog ? DifficultyLevel.Hard : DifficultyLevel.Easy;
    }
}
