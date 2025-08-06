using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car movement")]
    public static CarController instance;
    [SerializeField] private Rigidbody theRB;
    private CameraSwitch camSwitch;
    private bool ReverseCamActive = false;
   [SerializeField] private float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, 
        turnStrength = 180, gravityForce = 10f, dragOnGround = 0.02f;
    private float speedInput, turnInput;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float accelerationRate = 1.2f;
    [SerializeField] private float decelerationRate = 0.01f;
    [SerializeField] private bool grounded;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundRayLength = 0.5f;
    [SerializeField] private Transform groundRayPoint;
    [SerializeField] private ParticleSystem SpeedLines;
    [SerializeField] private float speedeffecttrigger = 15f;
    
    [Header("Engine Sound")]
    [SerializeField] private AudioSource engine;
    [SerializeField] private float minPitch = 0.2f;
    [SerializeField] private float maxPitch = 2.0f;
   
    public bool canMove = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        theRB.transform.parent = null; //to prevent jittering movement of the car model
         camSwitch = FindObjectOfType<CameraSwitch>();
    }

    public void StopAudio()
    {
        engine.Stop(); // just to stop the audio when victory/ lose screen is on
    }
    private void Update()
    { if (Input.GetAxis("Vertical") < 0 && !ReverseCamActive )
        {
            Debug.Log("reverse cam active");
            camSwitch.switchCam(camSwitch.cam2);
            ReverseCamActive = true;
        }
        else if (Input.GetAxis("Vertical") > 0 && ReverseCamActive)
        {
            Debug.Log("Normal cam active");
            camSwitch.switchCam(camSwitch.cam1);
            ReverseCamActive = false;
        }
    }
    private void FixedUpdate()
    { 
        float speedPercent = Mathf.Clamp01(currentSpeed / maxSpeed);// changes the pitch  of the sound to simulate an engine running. I added it here instead of the sfx manager just because its easier to control
       float targetPitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);
       engine.pitch = Mathf.Lerp(engine.pitch, targetPitch, Time.fixedDeltaTime * 2.5f);
        //Debug.Log(currentSpeed);
        if (currentSpeed > speedeffecttrigger)
        {
            SpeedLines.Play();// this is for the particles system which im using as speedlines
        }
        else
        {
            if(SpeedLines.isPlaying)
                SpeedLines.Stop();
        }
        if (canMove){
            grounded = false;
            RaycastHit hit;
            if (Physics.Raycast(groundRayPoint.position, -transform.up,
                    out hit, groundRayLength, whatIsGround)) //Checks if the raycast is hitting the ground
            {
                grounded = true;
                Console.WriteLine("You're Hitting the ground OG");
                //This is to make it so the car isn't flat and actually tilts to interact with the terian and surface it is on. For ramps it will tilt upwards etc etc
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation; 
            }
            speedInput = 0f;
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0) 
            {
                float acceleration = Input.GetAxis("Vertical") > 0 ? forwardAccel : reverseAccel; // this is a condensend if statement, technically saying if the input axis is greater than 0 its  will use foward accle other than that it will use reverse accel
                speedInput = Input.GetAxis("Vertical") * acceleration * 10f;
            }
            else // if theres no input, the cars movment will lerp and slow down
            {
                speedInput = Mathf.Lerp(speedInput, 0, decelerationRate * 0.1f * Time.deltaTime);
                theRB.linearDamping = 0.05f;
            }

// using lerp for smoother acceleration
            currentSpeed = Mathf.Lerp(currentSpeed, speedInput, accelerationRate * Time.deltaTime);
            turnInput = Input.GetAxis("Horizontal");
            if (grounded) //only accepts input if on the ground
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput *
                    turnStrength * Time.deltaTime ,
                    0f)); //Rather than add a force left and right, we rotate the object left andright and continue it's forwards of backwards motion
            }

            transform.position = theRB.transform.position;
            if (grounded)//only accepts input if on the ground
            {
                theRB.linearDamping = dragOnGround;
                theRB.velocity = Vector3.Lerp(theRB.velocity, transform.forward * currentSpeed, accelerationRate * Time.deltaTime);
            }
            else if (!grounded)
            {
                theRB.linearDamping = 0.01f;
                float pushdown = -gravityForce * 90f;
                theRB.AddForce(Vector3.up * pushdown); //pushes it down when the car is not on the ground.
            }

        }
    }
}