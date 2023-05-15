using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsShipController : MonoBehaviour
{
    public float force;
    public float rotationSpeed;
    private bool applyForce;
    private bool aimedCorrectly;
    //private Rigidbody hull;
    public bool reachedTarget = false;

    public Rigidbody rb;
    public Rigidbody otherShip;
    public float otherShipSpeed;
    public float combinedRelativeSpeed;
    public float approachSpeed;


    public float maxSpeed;
    public float maxAcceleration;
    public float maxRotation;

    public float orderedSpeed;
    public float orderedAcceleration;
    public float orderedRotation;

    public float currentSpeed;
    public float currentForwardSpeed;
    private Vector3? currentTargetCoordinates;//the target location
    private Quaternion currentTargetRotation;//the direction of the target relative to self

    public Vector3 cpa;
    public GameObject cpaSpherePrefab;
    public GameObject activeCPASphere;
    public float timeToCPA;
    public float distanceToCPA;
    public float distanceAtCPA;

    public bool sidewaysMotionDetected = false;
    public bool verticalMotionDetected = false;

    // Start is called before the first frame update
    void Start()
    {
        //hull = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = rb.velocity.magnitude;
        //otherShipSpeed = otherShip.velocity.magnitude;
        //Vector3 combinedVelocity = hull.velocity - otherShip.velocity;
        //combinedRelativeSpeed = ShipMathUtilities.CalculateCombinedSpeed(hull, otherShip);
        //approachSpeed = ShipMathUtilities.CalculateApproachRate(hull, otherShip);
        currentForwardSpeed = ShipMathUtilities.CalculateForwardVelocity(rb);

        //if(Input.GetKeyDown(KeyCode.V))
        //{          
        //    Destroy(activeCPASphere);
        //    Debug.Log("Pressed key");
        //    cpa = ShipMathUtilities.CalculateCPAForSelf(hull, otherShip);
        //    timeToCPA = ShipMathUtilities.CalculateTimeToCPA(hull, otherShip);
        //    activeCPASphere = Instantiate(cpaSpherePrefab, cpa, Quaternion.identity);
        //    activeCPASphere.SetActive(true);
        //}

        //if(timeToCPA > 0)
        //{
        //    timeToCPA = ShipMathUtilities.CalculateTimeToCPA(hull, otherShip);
        //    distanceToCPA = ShipMathUtilities.DistanceToCPA(hull, cpa);
        //    distanceAtCPA = ShipMathUtilities.DistanceAtCPA(hull, otherShip);
        //}

        //turn left and right with arrow keys (or A and D)
        //float horizontalInput = Input.GetAxis("Horizontal");
        //float rotationAngle = horizontalInput * rotationSpeed * Time.deltaTime;

        //transform.Rotate(Vector3.up, rotationAngle);

        if(currentTargetCoordinates.HasValue)
        {
            UpdateTargetDirection(currentTargetCoordinates);
        }

        if(currentTargetCoordinates.HasValue && transform.rotation != currentTargetRotation)
        {
            RotateShip(currentTargetCoordinates);
            
        }      

        if (transform.rotation == currentTargetRotation)
        {
            aimedCorrectly = true;
        } else
        {
            aimedCorrectly = false;
        }

        if(Vector3.Distance(transform.position, (Vector3)currentTargetCoordinates) < 0.1f)
        {
            currentTargetCoordinates = null;
            reachedTarget = true;
        }
    }

    private void FixedUpdate()
    {
        if(applyForce)
        {
            if(currentForwardSpeed < orderedSpeed/* && aimedCorrectly*/)
            {
                Debug.Log("Adding Thrust");
                rb.AddRelativeForce(0, 0, force);

                //push the ship to the right by adding force to the left
                //Vector3 rightForce = hull.transform.right * force;
                //hull.AddRelativeForce(rightForce, ForceMode.Force);

                //push the ship to the left by adding negative force to the left
                //Vector3 leftForce = hull.transform.right * force;
                //hull.AddRelativeForce(-leftForce, ForceMode.Force);

                //push the ship up by adding force to the bottom
                //Vector3 upForce = hull.transform.up * force;
                //hull.AddRelativeForce(upForce, ForceMode.Force);

                //push the ship down by adding negative force to the bottom
                //Vector3 downForce = hull.transform.up * force;
                //hull.AddRelativeForce(-upForce, ForceMode.Force);

                //Vector3 localLeft = -hull.transform.right;
                //Quaternion currentRotation = hull.rotation;
                //Vector3 worldLeft = currentRotation * localLeft;
                //hull.AddRelativeForce(worldLeft * force, ForceMode.Force);
            }
            if(aimedCorrectly)
            {
                CounterLateralMotion();
            }
            
        }                    
    }

    public void MoveShip(Vector3 target, float speed, float acceleration, float rotation)
    {
        Debug.Log("Speed = " + speed);
        currentTargetCoordinates = target;
        orderedSpeed = speed <= 0? maxSpeed : speed;
        //if(speed <= 0f)
        //{
        //    orderedSpeed = maxSpeed;
        //} else
        //{
        //    orderedSpeed = speed;
        //}
        Debug.Log("Ordered speed: " + orderedSpeed);
        orderedAcceleration = acceleration <= 0? acceleration : maxAcceleration;
        orderedRotation = rotation <= 0? rotation : maxRotation;
        applyForce = true;
    }

    public void RotateShip(Vector3? target)
    {
        Vector3 direction = ((Vector3)target - transform.position).normalized;
        currentTargetRotation = Quaternion.LookRotation(direction);

        //execute rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, currentTargetRotation, rotationSpeed * Time.deltaTime);
    }

    public void UpdateTargetDirection(Vector3? target)
    {
        Vector3 direction = ((Vector3)target - transform.position).normalized;
        currentTargetRotation = Quaternion.LookRotation(direction);
    }

    public void CounterLateralMotion()
    {
        float sidewaysMotion = Vector3.Dot(rb.velocity, transform.right);
        float verticalMotion = Vector3.Dot(rb.velocity, transform.up);

        if(ShipMathUtilities.CalculateAbsSidewaysMotion(rb) > 0.0001f)
        {
            //Debug.Log("Sideways motion detected: " + Mathf.Abs(sidewaysMotion));
            sidewaysMotionDetected = true;
            Vector3 correctiveDirection = -transform.right * sidewaysMotion;
            rb.AddRelativeForce(correctiveDirection * rotationSpeed * Time.deltaTime);
            Debug.Log("Applying sideways force: " + correctiveDirection.magnitude);
        } else
        {
            sidewaysMotionDetected = false;
        }
        if (ShipMathUtilities.CalculateAbsVerticalMotion(rb) > 0.0001f)
        {
            //Debug.Log("Vertical motion detected: " + Mathf.Abs(verticalMotion));
            verticalMotionDetected = true;
            Vector3 correctiveDirection = -transform.up * verticalMotion;
            rb.AddRelativeForce(correctiveDirection * rotationSpeed * Time.deltaTime);
            Debug.Log("Applying vertical force: " + correctiveDirection.magnitude);
        } else
        {
            verticalMotionDetected = false; ;
        }
    }

    public void TurnAndBurn(Vector3? target)
    {
        Vector3 direction = ((Vector3)target - transform.position).normalized;
        currentTargetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, currentTargetRotation, rotationSpeed * Time.deltaTime);
        Vector3 sidewaysForce = Vector3.Cross(Vector3.up, rb.velocity) * rotationSpeed;
        rb.AddRelativeForce(sidewaysForce, ForceMode.Force);
    }
    
}
