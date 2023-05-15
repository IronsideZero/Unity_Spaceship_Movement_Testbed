using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControllerMultiRB : MonoBehaviour
{
    public float force;
    public float rotationSpeed;
    private bool applyForce;
    private bool aimedCorrectly;
    private bool pointedInRightDirection = false;
    public bool reachedTarget = false;

    public Rigidbody hull;
    public Rigidbody otherShip;
    public float otherShipSpeed;
    public float combinedRelativeSpeed;
    public float approachSpeed;

    public Rigidbody mainEngines;
    public Rigidbody portThrusters;//on left, push ship right
    public Rigidbody starboardThrusters;//on right, push ship left
    public Rigidbody dorsalThrusters;//on top, push ship down
    public Rigidbody ventralThrusters;//on bottom, push ship up

    public bool applyUpForce = false;
    public bool applyDownForce = false;
    public bool applyLeftForce = false;
    public bool applyRightForce = false;


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

    public bool mayRotate = false;

    // Start is called before the first frame update
    void Start()
    {
        //hull = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = hull.velocity.magnitude;                
        currentForwardSpeed = ShipMathUtilities.CalculateForwardVelocity(hull);//in order for this to work correctly, the main engines and the overall object have to have the same rotation. the ship model may need to be rotated for this to look right. 
        /*
         * calculation block for a second ship
         */
        //otherShipSpeed = otherShip.velocity.magnitude;
        //Vector3 combinedVelocity = hull.velocity - otherShip.velocity;
        //combinedRelativeSpeed = ShipMathUtilities.CalculateCombinedSpeed(hull, otherShip);
        //approachSpeed = ShipMathUtilities.CalculateApproachRate(hull, otherShip);

        /*
         * Action block for dealing with a second ship
         */
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


        /*
         * Manual Control Block
         */

        //if (Input.GetKey(KeyCode.UpArrow))
        //    applyUpForce = true;
        //else
        //    applyUpForce = false;

        //if (Input.GetKey(KeyCode.DownArrow))
        //    applyDownForce = true;
        //else
        //    applyDownForce = false;

        //if (Input.GetKey(KeyCode.LeftArrow))
        //    applyLeftForce = true;
        //else
        //    applyLeftForce = false;

        //if (Input.GetKey(KeyCode.RightArrow))
        //    applyRightForce = true;
        //else
        //    applyRightForce = false;
        //if (Input.GetKey(KeyCode.Space))
        //    applyForce = true;
        //else
        //    applyForce = false;

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Pressing V");
            //LookAtDirectionOfTravel();
            mayRotate = true;
        }

        if (currentTargetCoordinates.HasValue)
        {
            //Debug.Log("CurrentTargetCoordinates has value: " + currentTargetCoordinates);
            UpdateTargetDirection(currentTargetCoordinates);
        }

        if (currentTargetCoordinates.HasValue && transform.rotation != currentTargetRotation)
        {
            RotateShip(currentTargetCoordinates);
            if(CalculateAngle() <= 30f && CalculateAngle() >= 0f)
            {
                pointedInRightDirection = true;
            } else
            {
                pointedInRightDirection= false;
                StopCalculatingLateralMovement();
            }
        }

        if (transform.rotation == currentTargetRotation)
        {
            aimedCorrectly = true;
        }
        else
        {
            aimedCorrectly = false;
        }

        if (Vector3.Distance(transform.position, (Vector3)currentTargetCoordinates) < 0.1f)
        {
            currentTargetCoordinates = null;
            reachedTarget = true;
        }

        if(currentForwardSpeed >= orderedSpeed)
        {
            applyForce = false;
        } else
        {
            applyForce= true;
        }
    }

    private void FixedUpdate()
    {
        //apply force to one or more directions
        //apply force to ship rear as main engines would, if applyForce is true and the ship is pointed to within 30 degrees of the correct vector
        if (applyForce && pointedInRightDirection)
        {
            //if (currentForwardSpeed < orderedSpeed/* && aimedCorrectly*/)
            //{
            //    Debug.Log("Adding Thrust");
            //    hull.AddRelativeForce(0, 0, force);

            //}
            //if (aimedCorrectly)
            //{
            //    CounterLateralMotion();
            //}
            //Debug.Log("Adding Forward Thrust");
            mainEngines.AddRelativeForce(0, 0, force);
            //CancelLateralMovement();
        }
        if(pointedInRightDirection)
        {
            CancelLateralMovement();
        } 
        if(applyUpForce)
        {
            //Debug.Log("Adding Up Thrust");
            ventralThrusters.AddRelativeForce(0, 0, force);
        }
        if(applyDownForce)
        {
            //Debug.Log("Adding Down Thrust");
            dorsalThrusters.AddRelativeForce(0, 0, force);
        }
        if(applyLeftForce)
        {
            //Debug.Log("Adding LEft Thrust");
            portThrusters.AddRelativeForce(0, 0, force);
        }
        if(applyRightForce)
        {
            //Debug.Log("Adding Right Thrust");
            starboardThrusters.AddRelativeForce(0, 0, force);
        }
        if(mayRotate)
        {
            //LookAtDirectionOfTravel();
            LookAwayFromDirectionOfTravel();
        }
        
        //calculate ship direction of movement
        //float sidewaysMotion = ShipMathUtilities.CalculateSidewaysMotionRaw(hull);
        //float verticalMotion = ShipMathUtilities.CalculateVerticalMotionRaw(hull);

        //if (sidewaysMotion > 0.001f)
        //{
        //    Debug.Log("Ship is moving left");
        //}
        //if (sidewaysMotion < -0.001f)
        //{
        //    Debug.Log("Ship is moving right");
        //}
        //if (verticalMotion > 0.001f)
        //{
        //    Debug.Log("Ship is moving up");
        //}
        //if (verticalMotion < -0.001f)
        //{
        //    Debug.Log("Ship is moving down");
        //}

        
    }

    public void MoveShip(Vector3 target, float speed, float acceleration, float rotation)
    {
        //Debug.Log("Speed = " + speed);
        currentTargetCoordinates = target;
        orderedSpeed = speed <= 0 ? maxSpeed : speed;
        //Debug.Log("Ordered speed: " + orderedSpeed);
        orderedAcceleration = acceleration <= 0 ? acceleration : maxAcceleration;
        orderedRotation = rotation <= 0 ? rotation : maxRotation;
        applyForce = true;
    }

    public void RotateShip(Vector3? target)
    {
        //get old (current) angle
        Vector3 oldEulerAngles = transform.rotation.eulerAngles;

        Vector3 direction = ((Vector3)target - transform.position).normalized;
        currentTargetRotation = Quaternion.LookRotation(direction);

        //execute rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, currentTargetRotation, rotationSpeed * Time.deltaTime);

        Vector3 newEulerAngles = transform.rotation.eulerAngles;
        //CalculateDirection(oldEulerAngles, newEulerAngles);
        CalculateHorizontalDirection((Vector3)target);
        CalculateVerticalDirection((Vector3)target);

    }

    public void UpdateTargetDirection(Vector3? target)
    {
        //Debug.Log("Updating direction");
        Vector3 direction = ((Vector3)target - transform.position).normalized;
        currentTargetRotation = Quaternion.LookRotation(direction);
    }


    public void CalculateHorizontalDirection(Vector3 targetPos)
    {
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = targetPos - transform.position;

        targetDirection.y = 0f;
        targetDirection.Normalize();

        float angle = Vector3.SignedAngle(currentDirection, targetDirection, transform.up);

        if (angle > 0f)
        {
            //Debug.Log("Rotating to the right");
        }
        else if (angle < 0f)
        {
            //Debug.Log("Rotating to the left");
        }
    }

    public void CalculateVerticalDirection(Vector3 targetPos)
    {
        Vector3 currentDirection = transform.forward;
        Vector3 targetDirection = targetPos - transform.position;

        targetDirection.x = 0f;
        targetDirection.z = 0f;
        targetDirection.Normalize();

        float angle = Vector3.SignedAngle(currentDirection, targetDirection, transform.right);

        if (angle < 0f)
        {
            //Debug.Log("Rotating up");
        }
        else if (angle > 0f)
        {
            //Debug.Log("Rotating down");
        }
    }

    public float CalculateAngle()
    {
        Vector3 currentDirection = transform.forward;
        //Debug.Log("current direction is: " + currentDirection);

        Vector3 targetDirection = (Vector3)currentTargetCoordinates - transform.position;
        targetDirection.Normalize();

        float angle = Vector3.Angle(currentDirection, targetDirection);
        //Debug.Log("Angle is: " + angle);
        return angle;
    }

    public void LookAtDirectionOfTravel()//this works so long as it is called in FixedUpdate 
    {
        Vector3 velocity = hull.velocity;
        Vector3 direction = velocity.normalized;
        Quaternion dir = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, rotationSpeed * Time.deltaTime);
    }

    public void LookAwayFromDirectionOfTravel()//this works so long as it is called in FixedUpdate 
    {
        Vector3 velocity = hull.velocity;
        Vector3 direction = velocity.normalized;
        Quaternion dir = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, dir, rotationSpeed * Time.deltaTime);
    }

    public void CancelLateralMovement()
    {
        //calculate ship direction of movement
        float sidewaysMotion = ShipMathUtilities.CalculateSidewaysMotionRaw(hull);
        float verticalMotion = ShipMathUtilities.CalculateVerticalMotionRaw(hull);

        //Debug.Log("Sideways motion is: " + sidewaysMotion);
        //Debug.Log("Vertical motion is: " + verticalMotion);

        if (sidewaysMotion < 0.001f)
        {
            //Debug.Log("Ship is moving left, need to push right.");
            applyRightForce = true;
        } else
        {
            applyRightForce = false;
        }
        if (sidewaysMotion > -0.001f)
        {
            //Debug.Log("Ship is moving right, need to push left.");
            applyLeftForce = true;
        } else
        {
            applyLeftForce = false;
        }
        if (verticalMotion > 0.001f)
        {
            //Debug.Log("Ship is moving up, need to push down.");
            applyDownForce = true;
        } else
        {
            applyDownForce = false;
        }
        if (verticalMotion < -0.001f)
        {
            //Debug.Log("Ship is moving down, need to push up.");
            applyUpForce = true;
        } else
        {
            applyUpForce = false;
        }
    }

    public void StopCalculatingLateralMovement()
    {
        applyUpForce = false;
        applyDownForce=false;
        applyLeftForce=false;
        applyRightForce=false;
    }


}
