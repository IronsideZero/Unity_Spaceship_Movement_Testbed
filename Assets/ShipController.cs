using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float maxSpeed = 10f; // The maximum speed of the ship
    public float maxAcceleration = 5f; // The maximum acceleration the ship can sustain. This is the same as deceleration. 

    //Here, 1 represents 100%. A max speed of 10f, and an ordered speed of 1 means that the ship will travel at 100% of max speed. 
    public int orderedAcceleration = 1;
    public int orderedSpeed = 1;
    

    public float rotationSpeed = 50f; // The speed at which the ship rotates

    private Vector3 targetPosition; // The position that the ship should move towards
    private Vector3 startingPosition;//the position the ship is in at the moment it starts moving
    private Vector3 shipCurrentPosition;
    public float distanceToTargetPos;    
    private Quaternion targetRotation; // The rotation that the ship should rotate towards
    private Quaternion oppRotation;//not used?
    
    private float currentSpeed = 0f; // The current speed of the ship
    private float targetSpeed = 0f; //the speed the ship is trying to get to, using acceleration or deceleration


    //bools to keep track of various ship states
    private bool isMoving = false; // Whether or not the ship is currently moving
    private bool isRotating = false; // Whether or not the ship is currently rotating
    private bool rotationComplete = false;
    private bool isAccelerating = false;
    private bool accelerationPhaseComplete = false;
    private bool isDecelerating = false;
    private bool enginesFiring = false;
    private bool thrustersFiring = false;
    private bool reachedTarget = false;

    //variables for calculating the distances at which the ship should start decelerating
    private float distToTarget;
    private float timeToReachTarget;
    private float requiredDeceleration;
    private float decelerationDistance;
    private float distOfAcceleration;
    private float distOfDeceleration;
    private float distAtSpeed;

    //bools used to mark certain critical events having occurred the first time
    private bool initialCalculationsMade = false;
    private bool reachedFullSpeed = false;
    private bool beginningDecelerationBurn = false;
    private bool flippedBackwards = false;
    private bool timeToFlip = false;

    //debugging properties

    private float distTravelledDuringAccel;
    private float distTravelledDuringDecel;
    private float distTravelledAtSpeed;
    private float travelledSoFar;

    private Vector3 positionAtFullSpeed;
    private Vector3 positionStartingDecel;
    private Vector3 positionAtZero;

    private bool targetZero = false;
    private float currentRotation = 0f;

    //may be used to calculate the angle from starting position to target, so as to determine the angle at which to continue moving afterwards
    private Vector3 directionToTarget;
    private float angleToTarget;

    // Method called when the ship should move
    public void MoveShip(Vector3 targetPos)
    {
        targetPosition = targetPos;
        isMoving = true;
        accelerationPhaseComplete = false;
        reachedTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPosition.x == 0 && targetPosition.y == 0 && targetPosition.z == 0)
            targetZero = true;
        else
            targetZero = false;

        distanceToTargetPos = Vector3.Distance(transform.position, targetPosition);
        if (isMoving)
        {
            if (!reachedTarget)
            {

                if(currentSpeed == 0)
                {
                    RotateShipFromRest();
                }
                

                //do math
                if (!initialCalculationsMade && !targetZero)
                {
                    InitialCalculations();
                }

                //acceleration phase
                if (!isRotating && rotationComplete && !accelerationPhaseComplete && !targetZero)
                {
                    Accelerate();
                }

                travelledSoFar = Vector3.Distance(shipCurrentPosition, startingPosition);

                //Steady speed phase. acceleration is done, ship is at target speed (currently max speed)
                if (currentSpeed == maxSpeed && distanceToTargetPos > distOfDeceleration && !targetZero && !timeToFlip)
                {
                    Cruise();
                }

                //Deceleration phase. once distance to target gets down to the pre-calculated distance needed to decelerate
                if (distanceToTargetPos <= distOfDeceleration + 10 && !targetZero)
                {
                    timeToFlip = true;
                    CruiseBackwards();
                }

                //Deceleration phase. once distance to target gets down to the pre-calculated distance needed to decelerate
                if (distanceToTargetPos <= distOfDeceleration && !targetZero && flippedBackwards)
                {
                    Decelerate();
                }

                if (distanceToTargetPos <= 0.1f && !targetZero)
                {
                    targetPosition = Vector3.zero;
                    reachedTarget = true;
                    //Stop();                
                }
            }

            if(targetZero && reachedTarget)
            {
                ContinueVector();
            }

        }
    }

    /**
     * Conduct some initial calculations to be used by the controller
     */
    public void InitialCalculations()
    {
        distOfAcceleration = (maxSpeed * maxSpeed - currentSpeed * currentSpeed) / (2 * maxAcceleration);
        distOfDeceleration = (maxSpeed * maxSpeed - 0f * 0f) / (2 * maxAcceleration);
        distAtSpeed = distanceToTargetPos - distOfDeceleration - distOfAcceleration;
        startingPosition = transform.position;
        initialCalculationsMade = true;
    }

    /**
     * This method only works when the ship is at rest. It rotates the ship towards the target before the ship begins to move. 
     */
    public void RotateShipFromRest()
    {
        Debug.Log("Rotate from rest");
        
        //set bools
        isRotating = true;
        thrustersFiring = true;

        //make calculations
        Vector3 direction = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);
        
        //execute rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //check completion and set bools
        if (transform.rotation == targetRotation)
        {
            isRotating = false;
            thrustersFiring = false;
            rotationComplete = true;
        }
    }

    /**
     * This method rotates the ship while the ship is already in motion. The ship continues to move in the previous direction while it rotates. This assumes no additional thrust from the engines. 
     */
    public void RotateShipWhileMoving()
    {
        //set bools
        isRotating = true;
        thrustersFiring = true;   
        
        //make calculations
        Vector3 direction = (targetPosition - transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);

        //execute rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //check completeness and set bools
        if (transform.rotation == targetRotation)
        {
            isRotating = false;
            thrustersFiring = false;
            rotationComplete = true;
        }

        //continue moving forward
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, maxAcceleration * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    public void Accelerate()
    {
        //set bools and current position
        isAccelerating = true;
        enginesFiring = true;
        shipCurrentPosition = transform.position;

        Debug.Log("Accelerating to target. Current speed is " + currentSpeed + " and current target is " + targetPosition);

        //set speed based on acceleration
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, maxAcceleration * Time.deltaTime);
        //transform.position += transform.forward * currentSpeed * Time.deltaTime;

        //calculate direction to travel and execute maneuver
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        transform.position += targetDirection * currentSpeed * Time.deltaTime;
    }

    public void Cruise()
    {        
        //marking how much distance was covered during acceleration
        if (!reachedFullSpeed)
        {
            distTravelledDuringAccel = Vector3.Distance(shipCurrentPosition, startingPosition);
            positionAtFullSpeed = shipCurrentPosition;
            reachedFullSpeed = true;
        }

        //set bools
        isAccelerating = false;
        accelerationPhaseComplete = true;
        enginesFiring = false;

        Debug.Log("Cruising to target. Current speed is " + currentSpeed);

        //currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, maxAcceleration * Time.deltaTime);
        //transform.position += transform.forward * currentSpeed * Time.deltaTime;
        
        //calculate direction to travel and continue to execute maneuver at current speed
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        transform.position += targetDirection * currentSpeed * Time.deltaTime;
    }

    public void CruiseBackwards()
    {
        //set bools
        isRotating = true;
        thrustersFiring = true;

        //marking whether ship has flipped
        if (!flippedBackwards)
        {
            Debug.Log("Working on flipping 180 degrees");            
        }
        //Quaternion oppositeTargetRotation = Quaternion.LookRotation(-Vector3.forward);
        Quaternion oppositeTargetRotation = Quaternion.Inverse(transform.rotation);//doesn't work, not using
        oppRotation = oppositeTargetRotation;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, oppositeTargetRotation, rotationSpeed * Time.deltaTime);
        
        //rotate ship by fixed amount of 180. Will need to be variable for other amounts. 
        if(currentRotation < 180f)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            currentRotation += rotationAmount;
        }
        //float won't be exactly 180
        if (currentRotation < 180.5f && currentRotation > 179.5f)
            currentRotation = 180f;
        
        //check completeness and set bools
        if(currentRotation == 180f)
        {
            flippedBackwards = true;
            isRotating = false;
            thrustersFiring = false;
        }
        Debug.Log("Continuing to fly backwards.");
        
        //calculate direction to travel and continue to execute maneuver
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        transform.position += targetDirection * currentSpeed * Time.deltaTime;

        //check completeness and set bools
        //if(transform.rotation == oppositeTargetRotation)
        //{
        //    flippedBackwards = true;
        //    isRotating = false;
        //    thrustersFiring = false;
        //}
    }

    public void Decelerate()
    {
        //set bools
        isDecelerating = true;
        enginesFiring = true;

        //marking how much distance was covered while at full speed
        if (!beginningDecelerationBurn)
        {
            distTravelledAtSpeed = Vector3.Distance(shipCurrentPosition, positionAtFullSpeed);
            positionStartingDecel = shipCurrentPosition;
            beginningDecelerationBurn = true;            
        }

        if(currentSpeed > 0)
            Debug.Log("Decelerating. Current speed is " + currentSpeed);

        //currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, maxAcceleration * Time.deltaTime);//to come to a halt
        currentSpeed = Mathf.MoveTowards(currentSpeed, 1.5f, maxAcceleration * Time.deltaTime);//to come to a pre-set speed. Will need to be a variable later. 
        //transform.position += transform.forward * currentSpeed * Time.deltaTime;//only works for ship going forward
        
        //calculate direction to travel and execute deceleration maneuver
        Vector3 targetDirection = (targetPosition - transform.position).normalized;
        transform.position += targetDirection * currentSpeed * Time.deltaTime;
    }

    public void ContinueVector()
    {
        Debug.Log("Continuing vector. Current speed is " + currentSpeed + " and current target is " + targetPosition);
        
        
        
        
        
        //keep ship moving exactly backwards at a constant speed
        transform.Translate(-Vector3.forward * currentSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        //marking how much distance was covered while decelerating
        if (!reachedTarget)
        {
            distTravelledDuringDecel = Vector3.Distance(shipCurrentPosition, positionStartingDecel);
            //positionStartingDecel = shipCurrentPosition;
            reachedTarget = true;
            Debug.Log("Reached destination. Current speed is " + currentSpeed);
        }

        targetPosition = shipCurrentPosition;
        reachedTarget = true;
    } 


    /**
     * This method is called when the ship is in motion and you want to give it a new target and make a gradual turn to point at it
     */
    public void AlterVectorWithArc()
    {

    }

    /**
     * This method is called when the ship is in motion and you want to give it a new target. The ship will continue on course and speed, and flip 180 degrees, and then decelerate to zero, and accelerate again on the new vector
     */
    public void AlterVectorWithFlip()
    {

    }
}
