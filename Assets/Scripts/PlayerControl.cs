using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    public float mouseNeutralZone = 0.2f;
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

    private TextMeshProUGUI textInput;
    private int currentScore = 0;
    private bool isJoystickActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 2;

        healthBar = GameObject.FindGameObjectsWithTag("healthBar")[0].GetComponent<HealthBar>();
        // TODO: fix
        // healthBar.HealthPointsMax = maxHP;
        // healthBar.HealthPoints = maxHP;

        textInput = GameObject.FindGameObjectsWithTag("text")[0].GetComponent<TextMeshProUGUI>();
    }

    public void IncreaseScore()
    {
        currentScore += 1;
        textInput.text = currentScore.ToString();
    }

    void FixedUpdate()
    {
        isControlled = false;

        // Inputs reading
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        // Previous controls
        // mouseXValue = Mathf.Clamp(Input.mousePosition.x / Screen.width, 0, 1) - 0.5f;
        // mouseYValue = Mathf.Clamp(Input.mousePosition.y / Screen.height, 0, 1) - 0.5f;

        RectTransform joystick = GameObject.FindGameObjectsWithTag("joystick")[0].GetComponent<RectTransform>();
        float x = joystick.localPosition.x;
        float y = joystick.localPosition.y;
        RectTransform joystickParent = joystick.transform.parent.GetComponent<RectTransform>();

        Vector2 direction = new Vector2(Input.mousePosition.x - joystickParent.position.x,
            Input.mousePosition.y - joystickParent.position.y);

        float angle = Mathf.Atan2(direction.y, direction.x);
        float newX = Mathf.Cos(angle);
        float newY = Mathf.Sin(angle);

        float scaler = 700f / Screen.width;
        float magSclaer = 50 / scaler;
        float magnitude = Mathf.Min(direction.magnitude, magSclaer);

        // Activate joystick after the first touch
        if (magnitude < magSclaer) {
            isJoystickActivated = true;
        } else {
            isJoystickActivated = false;
        }
        
        if (isJoystickActivated) {
            Vector2 newJoystickPos = new Vector2(newX, newY) * magnitude * scaler;
            joystick.localPosition = newJoystickPos;

            newJoystickPos /= 50f;
            mouseYValue = newJoystickPos.y;
            mouseXValue = newJoystickPos.x;
        } else {
            joystick.localPosition = Vector2.zero;
        }

        // Forward rotation restriction 
        float xEulerRotation = Mathf.Sin(transform.eulerAngles.x * 2 * Mathf.PI /  360);
        if (difficulty == DifficultyLevel.Easy) {
            if (xEulerRotation > maximumXRotation) {
                mouseYValue = Mathf.Min(mouseYValue, 0);
            } else if (xEulerRotation < -maximumXRotation) {
                mouseYValue = Mathf.Max(mouseYValue, 0);
            }
        }

        // Vertical input
        if (Mathf.Abs(verticalInput) > 0.01f) {
            rb.AddForce(transform.up * verticalInput * verticalForseMultiplier);
            isControlled = true;
        }

        // Horizontal input
        if (Mathf.Abs(horizontalInput) > 0.01f) {
            // // OR rb.AddRelativeForce(Vector3.forward..)
            // rb.AddTorque(-transform.forward * horizontalInput * horizontalForseMultiplier);
            // isControlled = true;
            rb.AddTorque(transform.up * horizontalInput * horizontalForseMultiplier);
            isControlled = true;
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
        } 

        // Mouse horizontal input
        if (Mathf.Abs(mouseXValue) > mouseNeutralZone) {
            // rb.AddTorque(transform.up * mouseXValue * mouseXMultiplier);
            // isControlled = true;
            // OR rb.AddRelativeForce(Vector3.forward..)
            rb.AddTorque(-transform.forward * mouseXValue * mouseXMultiplier);
            isControlled = true;
        }

        // Some small constant force to the UP direction
        rb.AddForce(Vector3.up * airUpForce);

        // Reset player
        if (Input.GetKey(KeyCode.Space)) {
            if (isTouchingGround) {
                // Reset rotation position when on the ground
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            } else {
                // Stabilize
                isControlled = false;
            }
        }

        // Helping stabilization when there are no inputs control
        if (!isControlled && difficulty == DifficultyLevel.Easy) {
            Vector3 predictedUp = Quaternion.AngleAxis(
                rb.angularVelocity.magnitude * Mathf.Rad2Deg * stabilityForce / stabilitySpeed,
                rb.angularVelocity
            ) * transform.up;
            Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
            rb.AddTorque(torqueVector * stabilitySpeed * stabilitySpeed);
        }

        // Wind force
        if (inWindZone) {
            rb.AddForce(windZone.transform.forward *
                windZone.GetComponent<WindZone>().windMain);
        }

        // It would be updated by event before each "Update" call
        isTouchingGround = false;

        if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadScene("MenuScene");
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
                if (healthBar.HealthPoints <= 0) {
                    EndGame();
                }
            } else {
                // Do some VFX
            }
        }

        
    }

    public void DifficultyToggle(bool tog) {
        difficulty = tog ? DifficultyLevel.Hard : DifficultyLevel.Easy;
    }

    public void EndGame()
    {
        rb.isKinematic = true;
        Invoke("RestartGame", 2f);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
